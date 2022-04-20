using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Actions;
using tech.aerove.streamdeck.client.Events;

namespace tech.aerove.streamdeck.client.Pipeline.Middleware
{
    /// <summary>
    /// Holds all incoming event messages at start until the global settings are
    /// Recieved. This ensures all data is available when actions are called.
    /// </summary>
    internal class EventOrderingMiddleware : MiddlewareBase
    {
        private readonly ILogger<EventOrderingMiddleware> _logger;
        private readonly IElgatoDispatcher _dispatcher;
        public EventOrderingMiddleware(ILogger<EventOrderingMiddleware> logger, IElgatoDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }
        private bool FirstEventHandled = false;
        private bool AwaitingInitialGlobalSettings = true;
        private List<IElgatoEvent> PendingMessages = new List<IElgatoEvent>();
        public override async Task HandleIncoming(IElgatoEvent message)
        {
            if (!FirstEventHandled)
            {
                _logger.LogDebug("Recieved first message. Holding messages until global settings are received!");
                FirstEventHandled = true;
                await _dispatcher.GetGlobalSettingsAsync();
            }
            if (AwaitingInitialGlobalSettings && message.Event != ElgatoEventType.DidReceiveGlobalSettings)
            {
                PendingMessages.Add(message);
                return;
            }
            await NextDelegate.InvokeNextIncoming(message);
            if (AwaitingInitialGlobalSettings)
            {
                _logger.LogDebug("Received global settings! Releasing {pendingmessagecount} pending messages.", PendingMessages.Count());
                AwaitingInitialGlobalSettings = false;
                foreach (var pendingmessage in PendingMessages)
                {
                    await NextDelegate.InvokeNextIncoming(pendingmessage);
                }
                PendingMessages.Clear();
            }
        }

        public override Task HandleOutgoing(JObject message)
        {
            return NextDelegate.InvokeNextOutgoing(message);
        }
    }
}
