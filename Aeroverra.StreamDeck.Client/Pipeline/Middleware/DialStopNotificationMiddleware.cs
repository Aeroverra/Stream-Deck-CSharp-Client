using Aeroverra.StreamDeck.Client.Events;
using Aeroverra.StreamDeck.Client.Events.SDKEvents;
using Aeroverra.StreamDeck.Client.Events.SharedModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Pipeline.Middleware
{
    /// <summary>
    /// Emits DialStop events after dial rotation pauses for configurable delays.
    /// </summary>
    internal sealed class DialStopNotificationMiddleware(ILogger<DialStopNotificationMiddleware> logger) : MiddlewareBase
    {
        private static readonly (DialStopEventLength Length, int DelayMs)[] StopSchedule = new[]
        {
            (DialStopEventLength.Short, 500),
            (DialStopEventLength.Medium, 1000),
            (DialStopEventLength.Long, 2000),
            (DialStopEventLength.ExtraLong, 3000),
        };

        private readonly object SyncRoot = new object();
        private readonly Dictionary<string, CancellationTokenSource> PendingStops = new Dictionary<string, CancellationTokenSource>();
        private readonly Dictionary<string, RotationState> RotationStates = new Dictionary<string, RotationState>();

        public override Task HandleIncoming(IElgatoEvent message)
        {
            switch (message)
            {
                case DialRotateEvent dialRotateEvent:
                    ScheduleDialStopNotifications(dialRotateEvent);
                    break;
                case WillDisappearEvent willDisappearEvent:
                    ClearPending(willDisappearEvent);
                    break;
            }

            return NextDelegate.InvokeNextIncoming(message);
        }

        public override Task HandleOutgoing(JObject message)
        {
            return NextDelegate.InvokeNextOutgoing(message);
        }

        private void ScheduleDialStopNotifications(DialRotateEvent dialRotateEvent)
        {
            var key = GetActionKey(dialRotateEvent);
            var cancellationTokenSource = new CancellationTokenSource();

            lock (SyncRoot)
            {
                if (RotationStates.TryGetValue(key, out var rotationState))
                {
                    rotationState.TotalTicks += dialRotateEvent.Payload.Ticks;
                    rotationState.LastPayload = dialRotateEvent.Payload;
                }
                else
                {
                    RotationStates[key] = new RotationState
                    {
                        TotalTicks = dialRotateEvent.Payload.Ticks,
                        LastPayload = dialRotateEvent.Payload
                    };
                }

                if (PendingStops.TryGetValue(key, out var existing))
                {
                    existing.Cancel();
                    existing.Dispose();
                    PendingStops.Remove(key);
                }

                PendingStops[key] = cancellationTokenSource;
            }

            foreach (var (length, delayMs) in StopSchedule)
            {
                var isFinalEvent = length == DialStopEventLength.ExtraLong;
                _ = PublishAfterDelay(dialRotateEvent, length, delayMs, key, cancellationTokenSource, isFinalEvent);
            }
        }

        private void ClearPending(IActionEvent actionEvent)
        {
            var key = GetActionKey(actionEvent);

            lock (SyncRoot)
            {
                if (PendingStops.TryGetValue(key, out var pending))
                {
                    pending.Cancel();
                    pending.Dispose();
                    PendingStops.Remove(key);
                    RotationStates.Remove(key);
                }
            }
        }

        private async Task PublishAfterDelay(
            DialRotateEvent source,
            DialStopEventLength eventLength,
            int delayMs,
            string key,
            CancellationTokenSource cancellationTokenSource,
            bool isFinalEvent)
        {
            RotationState? rotationState = null;

            lock (SyncRoot)
            {
                if (PendingStops.TryGetValue(key, out var existing) && ReferenceEquals(existing, cancellationTokenSource))
                {
                    RotationStates.TryGetValue(key, out rotationState);
                }
            }

            if (rotationState == null)
            {
                return;
            }

            try
            {
                await Task.Delay(delayMs, cancellationTokenSource.Token);

                var payload = ClonePayload(rotationState.LastPayload, rotationState.TotalTicks);

                var dialStopEvent = new DialStopEvent
                {
                    SDKId = source.SDKId,
                    Event = ElgatoEventType.DialStop,
                    Action = source.Action,
                    Context = source.Context,
                    Device = source.Device,
                    Payload = payload,
                    MSDelay = delayMs,
                    EventLength = eventLength
                };

                var jObject = JObject.FromObject(dialStopEvent);
                dialStopEvent.RawJObject = jObject;
                dialStopEvent.Raw = jObject.ToString();

                await NextDelegate.InvokeNextIncoming(dialStopEvent);
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to send DialStop {EventLength} event after {Delay}ms for context {Context}.", eventLength, delayMs, source.Context);
            }
            finally
            {
                if (isFinalEvent)
                {
                    RemoveIfCurrent(key, cancellationTokenSource);
                }
            }
        }

        private void RemoveIfCurrent(string key, CancellationTokenSource cancellationTokenSource)
        {
            lock (SyncRoot)
            {
                if (PendingStops.TryGetValue(key, out var existing) && ReferenceEquals(existing, cancellationTokenSource))
                {
                    PendingStops.Remove(key);
                    existing.Dispose();
                    RotationStates.Remove(key);
                }
            }
        }

        private static string GetActionKey(IActionEvent actionEvent)
        {
            if (actionEvent.SDKId != Guid.Empty)
            {
                return actionEvent.SDKId.ToString();
            }

            return actionEvent.Context;
        }

        private static DialRotatePayload ClonePayload(DialRotatePayload source, int totalTicks)
        {
            return new DialRotatePayload
            {
                Controller = source.Controller,
                Coordinates = source.Coordinates,
                Resources = source.Resources,
                Settings = source.Settings,
                Pressed = source.Pressed,
                Ticks = totalTicks
            };
        }

        private sealed class RotationState
        {
            public int TotalTicks { get; set; }
            public required DialRotatePayload LastPayload { get; set; }
        }
    }
}
