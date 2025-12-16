using Aeroverra.StreamDeck.Client.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Pipeline.Middleware
{
    /// <summary>
    /// Gives each action a unique SDKId that persists across sessions.
    /// Also fixes a bug where the stream deck will sometimes sends <see cref="WillAppearEvent"/> out of order by delaying events until after <see cref="WillAppearEvent"/>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dispatcher"></param>
    internal sealed class IdSDKMiddleware(ILogger<IdSDKMiddleware> logger, IElgatoDispatcher dispatcher) : MiddlewareBase
    {
        private const int OutOfOrderEventDisposalSeconds = 5;

        private HashSet<string> KnownActionContextIds = new HashSet<string>();

        private readonly Dictionary<string, CancellationTokenSource> DelayedOutOfOrderEventExpirations = new Dictionary<string, CancellationTokenSource>();

        private readonly Dictionary<string, List<IElgatoEvent>> OutOfOrderActionEvents = new Dictionary<string, List<IElgatoEvent>>();

        private readonly Dictionary<string, Guid> ContextToSDKIds = new Dictionary<string, Guid>();

        private readonly Dictionary<Guid, string> SDKIdsToContext = new Dictionary<Guid, string>();

        public override async Task HandleIncoming(IElgatoEvent message)
        {
            // Did disapear event is usually fired after willdisapear...
            // I have considered tracking when its opened and then delaying the willdisapear until after
            // but when moving an action with the property inspector open it will often fire two willapear events for some reason followed by a single willdisapear event
            // and handling all that complexity for something that I don't currently see any use case for seems unnecessary.
            // so this will just have to be the one action event that may fire out of order..
            if (message is IActionEvent actionEvent && message is not PropertyInspectorDidDisappearEvent)
            {
                var knownAction = KnownActionContextIds.Contains(actionEvent.Context);

                if (knownAction == false)
                {
                    if (message is WillAppearEvent willAppearEvent)
                    {
                        await HandleWillAppearEvent(willAppearEvent);
                    }
                    else
                    {
                        await HandleOutOfOrderActionEvent(message);
                    }
                }
                else if (message is WillDisappearEvent willDisapearEvent)
                {
                    await HandleWillDisapearEvent(willDisapearEvent);
                }
                else
                {
                    await SendActionEventWithId(message);
                }
                return;
            }
            await NextDelegate.InvokeNextIncoming(message);
        }

        private async Task SendActionEventWithId(IElgatoEvent message)
        {
            if (message is IActionEvent actionEvent)
            {
                if (ContextToSDKIds.TryGetValue(actionEvent.Context, out var sdkId))
                {
                    actionEvent.SDKId = sdkId;
                }
                else
                {
                    if (message is WillAppearEvent willAppearEvent)
                    {
                        var sdkIdString = $"{willAppearEvent.Payload.Settings["sdkId"]}";
                        if (Guid.TryParse(sdkIdString, out sdkId))
                        {
                            if (SDKIdsToContext.TryGetValue(sdkId, out var existingContext))
                            {
                                logger.LogError("SDKId {SDKId} already associated with context {ExistingContext}. Reassigning to context {NewContext}.", sdkId, existingContext, actionEvent.Context);
                            }
                            actionEvent.SDKId = sdkId;
                            ContextToSDKIds[actionEvent.Context] = sdkId;
                            SDKIdsToContext[sdkId] = actionEvent.Context;
                        }
                        else
                        {
                            sdkId = Guid.NewGuid();
                            actionEvent.SDKId = sdkId;
                            ContextToSDKIds[actionEvent.Context] = sdkId;
                            SDKIdsToContext[sdkId] = actionEvent.Context;
                            willAppearEvent.Payload.Settings["sdkId"] = sdkId.ToString();
                            await dispatcher.SetSettingsAsync(actionEvent.Context, willAppearEvent.Payload.Settings);
                        }
                    }
                    else
                    {
                        logger.LogError("No SDKId found for context {Context} on event {Event}. Generating new SDKId.", actionEvent.Context, message.Event);
                    }
                   
                }
                await NextDelegate.InvokeNextIncoming(message);
            }
        }

        public override Task HandleOutgoing(JObject message)
        {
            return NextDelegate.InvokeNextOutgoing(message);
        }

        private async Task HandleWillDisapearEvent(WillDisappearEvent e)
        {
            KnownActionContextIds.Remove(e.Context);

            await SendActionEventWithId(e);
        }

        private async Task HandleWillAppearEvent(WillAppearEvent e)
        {
            KnownActionContextIds.Add(e.Context);

            await SendActionEventWithId(e);

            if (DelayedOutOfOrderEventExpirations.TryGetValue(e.Context, out var cancellationTokenSource2))
            {
                cancellationTokenSource2.Cancel();
                cancellationTokenSource2.Dispose();
                DelayedOutOfOrderEventExpirations.Remove(e.Context);
            }

            if (OutOfOrderActionEvents.TryGetValue(e.Context, out var pendingEvents))
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
                        await SendActionEventWithId(pendingEvent);
                    }
                }

                if (disapearEvent != null)
                {
                    logger.LogWarning("Processing queued out-of-order WillDisappear event for context {Context}.", e.Context);

                    await HandleWillDisapearEvent(disapearEvent);
                }
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
                await Task.Delay(TimeSpan.FromSeconds(OutOfOrderEventDisposalSeconds), cancellationTokenSource.Token);

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
