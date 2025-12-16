using Aeroverra.StreamDeck.Client.Events;
using Aeroverra.StreamDeck.Client.Events.SDKEvents;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;

namespace Aeroverra.StreamDeck.Client.Pipeline.Middleware
{
    /// <summary>
    /// Handles the lifecycle of actions by sending custom OnInitialized and Dispose events.
    /// </summary>
    internal class LifeCycleMiddleware(ILogger<LifeCycleMiddleware> logger) : MiddlewareBase
    {
        private const int DisposeDelaySeconds = 10;

        private HashSet<Guid> KnownSDKIds = new HashSet<Guid>();

        private readonly Dictionary<Guid, CancellationTokenSource> PendingDisposes = new Dictionary<Guid, CancellationTokenSource>();

        public override async Task HandleIncoming(IElgatoEvent message)
        {
            if (message is IActionEvent actionEvent)
            {
                var knownAction = KnownSDKIds.Contains(actionEvent.SDKId);

                if (message is WillAppearEvent willAppearEvent)
                {
                    await SendOnInitializedEvent(willAppearEvent);
                }
                else if (knownAction == true  && message is WillDisappearEvent willDisappearEvent)
                {
                    await SendDisposeEvent(willDisappearEvent);
                }
            }

            await NextDelegate.InvokeNextIncoming(message);
        }

        public override Task HandleOutgoing(JObject message)
        {
            return NextDelegate.InvokeNextOutgoing(message);
        }

        private async Task SendOnInitializedEvent(WillAppearEvent e)
        {
            // Cancel any pending dispose
            if (PendingDisposes.TryGetValue(e.SDKId, out var cancellationTokenSource))
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                PendingDisposes.Remove(e.SDKId);
            }

            if (KnownSDKIds.Contains(e.SDKId))
                return;

            KnownSDKIds.Add(e.SDKId);

            var onInitializeEvent = new OnInitializedEvent
            {
                SDKId = e.SDKId,
                Action = e.Action,
                Context = e.Context,
                Event = ElgatoEventType.OnInitialized,
                Payload = e.Payload,
                Device = e.Device,
            };

            var jObject = JObject.FromObject(onInitializeEvent);

            onInitializeEvent.RawJObject = jObject;

            onInitializeEvent.Raw = jObject.ToString();

            await NextDelegate.InvokeNextIncoming(onInitializeEvent);

        }

        private Task SendDisposeEvent(WillDisappearEvent e)
        {
            if (PendingDisposes.TryGetValue(e.SDKId, out var existing))
            {
                // already scheduled; ignore duplicates
                logger.LogError("Duplicate WillDisappear event received for context {Context}. Ignoring duplicate.", e.Context);
                return Task.CompletedTask;
            }

            var cancellationTokenSource = new CancellationTokenSource();

            PendingDisposes[e.SDKId] = cancellationTokenSource;

            _ = DelayedDisposeAsync(cancellationTokenSource, e);

            return Task.CompletedTask;
        }

        private async Task DelayedDisposeAsync(CancellationTokenSource cancellationTokenSource, WillDisappearEvent e)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(DisposeDelaySeconds), cancellationTokenSource.Token);

                KnownSDKIds.Remove(e.SDKId);
                cancellationTokenSource.Dispose();
                PendingDisposes.Remove(e.SDKId);

                var disposeEvent = new DisposeEvent
                {
                    SDKId = e.SDKId,
                    Action = e.Action,
                    Context = e.Context,
                    Event = ElgatoEventType.Dispose,
                    Payload = e.Payload,
                    Device = e.Device,
                };

                var jObject = JObject.FromObject(disposeEvent);

                disposeEvent.RawJObject = jObject;

                disposeEvent.Raw = jObject.ToString();

                await NextDelegate.InvokeNextIncoming(disposeEvent);
            }
            catch (TaskCanceledException)
            {
                // WillAppear happened → expected
                return;
            }
        }
    }
}
