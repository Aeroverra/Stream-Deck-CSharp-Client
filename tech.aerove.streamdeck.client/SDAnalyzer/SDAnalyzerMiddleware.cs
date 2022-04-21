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
using tech.aerove.streamdeck.client.Pipeline;
using tech.aerove.streamdeck.client.Pipeline.Middleware;

namespace tech.aerove.streamdeck.client.SDAnalyzer
{
    internal class SDAnalyzerMiddleware : MiddlewareBase
    {
        private readonly ILogger<EventLoggingMiddleware> _logger;
        private readonly SDAnalyzerService _analyzer;
        public SDAnalyzerMiddleware(ILogger<EventLoggingMiddleware> logger, SDAnalyzerService analyzer)
        {
            _logger = logger;
            _analyzer = analyzer;
        }
        public override Task HandleIncoming(IElgatoEvent message)
        {
            return NextDelegate.InvokeNextIncoming(message);
        }

        public override Task HandleOutgoing(JObject message)
        {
            return NextDelegate.InvokeNextOutgoing(message);
        }
    }
}
