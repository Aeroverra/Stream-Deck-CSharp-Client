using Aeroverra.StreamDeck.Client.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Pipeline.Middleware
{
    /// <summary>
    /// This is mostly an easter egg / my way of showing I created this Client
    /// </summary>
    internal class AeroverraMiddleware : MiddlewareBase
    {
        private readonly ILogger<EventLoggingMiddleware> _logger;
        public AeroverraMiddleware(ILogger<EventLoggingMiddleware> logger)
        {
            _logger = logger;
        }
        public override Task HandleIncoming(IElgatoEvent message)
        {
            if (message.Event == ElgatoEventType.DidReceiveGlobalSettings)
            {
                var e = (message as DidReceiveGlobalSettingsEvent);
                e.Payload.Settings.Remove("Aeroverra");
            }
            return NextDelegate.InvokeNextIncoming(message);
        }

        public override Task HandleOutgoing(JObject message)
        {
            if (message["event"]?.ToString() == "setGlobalSettings")
            {
                var e = message["payload"]!["Aeroverra"] = "Stream Deck Client SDK By Aeroverra. https://aero.vi";
            }
            return NextDelegate.InvokeNextOutgoing(message);
        }
    }
}
