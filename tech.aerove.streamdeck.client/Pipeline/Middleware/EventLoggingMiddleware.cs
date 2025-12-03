using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tech.Aerove.StreamDeck.Client.Events;

namespace Tech.Aerove.StreamDeck.Client.Pipeline.Middleware
{
    internal class EventLoggingMiddleware : MiddlewareBase
    {
        private readonly ILogger<EventLoggingMiddleware> _logger;
        private Formatting Format = Formatting.None;
        private readonly bool formatted;
        private readonly bool typeOnly;

        public EventLoggingMiddleware(ILogger<EventLoggingMiddleware> logger, IConfiguration config)
        {
            _logger = logger;
            typeOnly = config.GetValue<bool>("ElgatoEventLoggingTypeOnly", false);
            formatted = config.GetValue<bool>("ElgatoEventLoggingFormatted", false);
            if (formatted)
            {
                Format = Formatting.Indented;
            }

            //note: the code disables this at the service container not this file
            if (config["ElgatoEventLogging"] == null)
            {
                _logger.LogDebug("Elgato event logging is enabled. To disable this set 'ElgatoEventLogging' to false in your appsettings.\r\n" +
                    "For pretty json set 'ElgatoEventLoggingFormatted' to true.\r\n" +
                    "For event type only set ElgatoEventLoggingTypeOnly to true");
            }
        }
        public override Task HandleIncoming(IElgatoEvent message)
        {
            if (typeOnly)
            {
                _logger.LogDebug("[{direction}] Event Type: {eventtype}", "Received", message.Event);

            }
            else
            {
                _logger.LogDebug("[{direction}] Event Type: {eventtype} Data: {incomingevent}", "Received", message.Event, JsonConvert.SerializeObject(message.RawJObject, Format));

            }
            return NextDelegate.InvokeNextIncoming(message);
        }

        public override Task HandleOutgoing(JObject message)
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
            return NextDelegate.InvokeNextOutgoing(message);
        }
    }
}
