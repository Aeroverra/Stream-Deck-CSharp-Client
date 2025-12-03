using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Tech.Aerove.StreamDeck.Client.Events;

namespace Tech.Aerove.StreamDeck.Client.Pipeline.Middleware
{
    /// <summary>
    /// This is mostly an easter egg / my way of showing I created this Client
    /// </summary>
    internal class AeroveMiddleware : MiddlewareBase
    {
        private readonly ILogger<EventLoggingMiddleware> _logger;
        public AeroveMiddleware(ILogger<EventLoggingMiddleware> logger)
        {
            _logger = logger;
        }
        public override Task HandleIncoming(IElgatoEvent message)
        {
            if (message.Event == ElgatoEventType.DidReceiveGlobalSettings)
            {
                var e = (message as DidReceiveGlobalSettingsEvent);
                e.Payload.Settings.Remove("aerove");
            }
            return NextDelegate.InvokeNextIncoming(message);
        }

        public override Task HandleOutgoing(JObject message)
        {
            if (message["event"]?.ToString() == "setGlobalSettings")
            {
                var e = message["payload"]["aerove"] = "Stream Deck Client SDK By Aeroverra. Aerove.Tech";
            }
            return NextDelegate.InvokeNextOutgoing(message);
        }
    }
}
