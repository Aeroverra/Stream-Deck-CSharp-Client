using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
    internal class EventLoggingMiddleware : MiddlewareBase
    {
        private readonly ILogger<EventLoggingMiddleware> _logger;
        private Formatting Format = Formatting.None;
        public EventLoggingMiddleware(ILogger<EventLoggingMiddleware> logger, IConfiguration config)
        {
            _logger = logger;
            if (config.GetValue<bool>("ElgatoEventLoggingFormatted", false))
            {
                Format = Formatting.Indented;
            }
            if (config["ElgatoEventLogging"] == null || config["ElgatoEventLoggingFormatted"] == null)
            {
                _logger.LogDebug("Elgato event logging is enabled. To disable this set 'ElgatoEventLogging' to false in your appsettings. For pretty json set 'ElgatoEventLoggingFormatted' to true.");
            }
        }
        public override Task HandleIncoming(IElgatoEvent message)
        {
            _logger.LogDebug("[{direction}] Event Type: {eventtype} Data: {incomingevent}", "Received", message.Event, JsonConvert.SerializeObject(message, Format));
            return NextDelegate.InvokeNextIncoming(message);
        }

        public override Task HandleOutgoing(JObject message)
        {
            string eventType = $"{message["Event"]}";
            _logger.LogDebug("[{direction}] Event Type: {eventtype} Data: {outgoingevent}", "Sending", eventType, message.ToString(Format));
            return NextDelegate.InvokeNextOutgoing(message);
        }
    }
}
