using Aeroverra.StreamDeck.Client.Events;
using Aeroverra.StreamDeck.Client.Events.SDKEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Pipeline.Middleware
{
    /// <summary>
    /// Handles logging custom <see cref="ISDKEvent"/> messages to the logger."/>
    /// </summary>
    internal sealed class SDKEventLoggingMiddleware : MiddlewareBase
    {
        private readonly ILogger<EventLoggingMiddleware> _logger;
        private Formatting Format = Formatting.None;
        private readonly bool formatted;
        private readonly bool typeOnly;
        private readonly bool incoming;
        private readonly bool outgoing;

        public SDKEventLoggingMiddleware(ILogger<EventLoggingMiddleware> logger, IConfiguration config)
        {
            _logger = logger;
            incoming = config.GetValue<bool>("ElgatoEventLoggingIncoming", true);
            outgoing = config.GetValue<bool>("ElgatoEventLoggingOutgoing", true);
            typeOnly = config.GetValue<bool>("ElgatoEventLoggingTypeOnly", false);
            formatted = config.GetValue<bool>("ElgatoEventLoggingFormatted", false);
            if (formatted)
            {
                Format = Formatting.Indented;
            }
        }

        public override Task HandleIncoming(IElgatoEvent message)
        {
            if (incoming == true && message is ISDKEvent sdkEvent)
            {
                if (typeOnly)
                {
                    _logger.LogDebug("[{direction}] Event Type: {eventtype}", "Received", message.Event);

                }
                else
                {
                    _logger.LogDebug("[{direction}] Event Type: {eventtype} Data: {incomingevent}", "Received", message.Event, JsonConvert.SerializeObject(message.RawJObject, Format));

                }
            }
            return NextDelegate.InvokeNextIncoming(message);
        }

        public override Task HandleOutgoing(JObject message)
        {
            if (outgoing == true && message is ISDKEvent sdkEvent)
            {
                string eventType = $"{message["event"]}";
                if (typeOnly)
                {
                    _logger.LogDebug("[{direction}] Event Type: {eventtype}", "Sending", eventType);

                }
                else
                {
                    _logger.LogDebug("[{direction}] Event Type: {eventtype} Data: {outgoingevent}", "Sending", eventType, message.ToString(Format));

                }
            }
            return NextDelegate.InvokeNextOutgoing(message);
        }
    }
}
