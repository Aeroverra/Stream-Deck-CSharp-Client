using Aeroverra.StreamDeck.Client.Events;
using Aeroverra.StreamDeck.Client.Events.SDKEvents;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;

namespace Aeroverra.StreamDeck.Client.Pipeline.Middleware
{
    /// <summary>
    /// Handles the lifecycle of actions by sending custom OnInitialized and Dispose events.
    /// Also fixes a bug where the stream deck will sometimes sends <see cref="WillAppearEvent"/> out of order by delaying events until after <see cref="WillAppearEvent"/>
    /// </summary>
    internal class LifeCycleMiddleware(ILogger<LifeCycleMiddleware> logger) : MiddlewareBase
    {
        private HashSet<string> KnownActionContextIds = new HashSet<string>();

        private readonly Dictionary<string, CancellationTokenSource> PendingDisposes = new Dictionary<string, CancellationTokenSource>();

        private readonly Dictionary<string, CancellationTokenSource> DelayedOutOfOrderEventExpirations = new Dictionary<string, CancellationTokenSource>();

        private readonly Dictionary<string, List<IElgatoEvent>> OutOfOrderActionEvents = new Dictionary<string, List<IElgatoEvent>>();

        public override async Task HandleIncoming(IElgatoEvent message)
        {
            if (message is IActionEvent actionEvent)
            {
                var knownAction = KnownActionContextIds.Contains(actionEvent.Context);

                if (message is WillAppearEvent willAppearEvent)
                {
                    await SendOnInitializedEvent(willAppearEvent);
                }
                else if (knownAction == true  && message is WillDisappearEvent willDisappearEvent)
                {
                    await SendDisposeEvent(willDisappearEvent);
                }
                else if (knownAction == false)
                {
                    await HandleOutOfOrderActionEvent(message);
                    return;
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
            if (PendingDisposes.TryGetValue(e.Context, out var cancellationTokenSource))
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                PendingDisposes.Remove(e.Context);
            }

            if (KnownActionContextIds.Contains(e.Context))
                return;

            KnownActionContextIds.Add(e.Context);

            if (DelayedOutOfOrderEventExpirations.TryGetValue(e.Context, out var cancellationTokenSource2))
            {
                cancellationTokenSource2.Cancel();
                cancellationTokenSource2.Dispose();
            }

            var hasOutOfOrderActionEvents = OutOfOrderActionEvents.TryGetValue(e.Context, out var pendingEvents);

            var onInitializeEvent = new OnInitializedEvent
            {
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

            if (hasOutOfOrderActionEvents)
            {
                OutOfOrderActionEvents.Remove(e.Context);

                WillDisappearEvent? disapearEvent = null;
                foreach (var pendingEvent in pendingEvents!)
                {
                    logger.LogWarning("Processing queued out-of-order action event {ActionEvent}.", (pendingEvent as IElgatoEvent)!.Event);
                    if (pendingEvent is WillDisappearEvent willDisappearEvent)
                    {
                        if (disapearEvent != null)
                        {
                            // Not sure how this would be possible
                            logger.LogError("Multiple WillDisappear events queued for context {Context}.", e.Context);
                        }
                        disapearEvent = willDisappearEvent;
                    }
                    else
                    {
                        await NextDelegate.InvokeNextIncoming(pendingEvent);
                    }
                }

                if (disapearEvent != null)
                {
                    logger.LogWarning("Processing queued out-of-order WillDisappear event for context {Context}.", e.Context);
                    await SendDisposeEvent(disapearEvent);
                }
            }
        }

        private Task SendDisposeEvent(WillDisappearEvent e)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            PendingDisposes[e.Context] = cancellationTokenSource;

            _ = DelayedDisposeAsync(cancellationTokenSource, e);

            return Task.CompletedTask;
        }

        private async Task DelayedDisposeAsync(CancellationTokenSource cancellationTokenSource, WillDisappearEvent e)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationTokenSource.Token);

                KnownActionContextIds.Remove(e.Context);
                cancellationTokenSource.Dispose();
                PendingDisposes.Remove(e.Context);

                var disposeEvent = new DisposeEvent
                {
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

        private Task HandleOutOfOrderActionEvent(IElgatoEvent message)
        {
            if (message is IActionEvent actionEvent)
            {
                logger.LogWarning("Received out-of-order action event {ActionEvent} before OnInitialized. Queuing event.", message.Event);

                if (OutOfOrderActionEvents.TryGetValue(actionEvent.Context, out var pendingEvents))
                {
                    pendingEvents.Add(message);
                }
                else
                {
                    pendingEvents = new List<IElgatoEvent> { message };
                    OutOfOrderActionEvents[actionEvent.Context] = pendingEvents;
                }

                if (DelayedOutOfOrderEventExpirations.TryGetValue(actionEvent.Context, out var cancellationTokenSource))
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                }

                cancellationTokenSource = new CancellationTokenSource();

                DelayedOutOfOrderEventExpirations[actionEvent.Context] = cancellationTokenSource;

                _= DelayedOutOfOrderActionEvent(cancellationTokenSource, actionEvent.Context);

            }
            return Task.CompletedTask;
        }

        private async Task DelayedOutOfOrderActionEvent(CancellationTokenSource cancellationTokenSource, string context)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationTokenSource.Token);

                cancellationTokenSource.Dispose();
                DelayedOutOfOrderEventExpirations.Remove(context);

                if (OutOfOrderActionEvents.TryGetValue(context, out var pendingEvents))
                {
                    OutOfOrderActionEvents.Remove(context);

                    foreach (var pendingEvent in pendingEvents)
                    {
                        logger.LogError("Discarding out-of-order action event {ActionEvent} after timeout. No WillApearEvent received!", (pendingEvent as IElgatoEvent)!.Event);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // WillAppear happened → expected
                return;
            }
        }
    }
}
