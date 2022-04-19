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
        public EventLoggingMiddleware(ILogger<EventLoggingMiddleware> logger)
        {
            _logger = logger;
        }
        public override Task HandleIncoming(IElgatoEvent message)
        {
            _logger.LogDebug("Received Event Type: {eventtype} Data: {incomingevent}",message.Event, JsonConvert.SerializeObject(message));
            return NextDelegate.InvokeNextIncoming(message);
        }

        public override Task HandleOutgoing(object message)
        {
            var messageString = JsonConvert.SerializeObject(message);
            var jo =  JObject.Parse(messageString);
            string eventType = $"{jo["Event"]}";
            _logger.LogDebug("Sending Event Type: {eventtype} Data: {outgoingevent}", eventType, messageString);
            return NextDelegate.InvokeNextOutgoing(message);
        }
    }
}
