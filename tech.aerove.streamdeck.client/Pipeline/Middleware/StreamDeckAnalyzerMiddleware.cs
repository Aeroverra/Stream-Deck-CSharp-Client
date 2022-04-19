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
    internal class StreamDeckAnalyzerMiddleware : MiddlewareBase
    {
        private readonly ILogger<EventLoggingMiddleware> _logger;
        public StreamDeckAnalyzerMiddleware(ILogger<EventLoggingMiddleware> logger)
        {
            _logger = logger;
        }
        public override Task HandleIncoming(IElgatoEvent message)
        {
            return NextDelegate.InvokeNextIncoming(message);
        }

        public override Task HandleOutgoing(object message)
        {
            return NextDelegate.InvokeNextOutgoing(message);
        }
    }
}
