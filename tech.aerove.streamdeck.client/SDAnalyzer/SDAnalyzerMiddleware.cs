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
    /// <summary>
    /// This class relies on the event ordering middleware being in place to work properly
    /// </summary>
    internal class SDAnalyzerMiddleware : MiddlewareBase
    {
        private readonly ILogger<EventLoggingMiddleware> _logger;
        private readonly SDAnalyzerService _analyzer;
        public SDAnalyzerMiddleware(ILogger<EventLoggingMiddleware> logger, SDAnalyzerService analyzer)
        {
            _logger = logger;
            _analyzer = analyzer;
        }
        private bool FirstEventHandled = false;
        public override Task HandleIncoming(IElgatoEvent message)
        {
            var e = message.Event;
            if (!FirstEventHandled && e != ElgatoEventType.DidReceiveGlobalSettings)
            {
                return NextDelegate.InvokeNextIncoming(message);
            }
            if (!FirstEventHandled && e == ElgatoEventType.DidReceiveGlobalSettings)
            {
                FirstEventHandled = true;
                _analyzer.HandleFirstLoad((DidReceiveGlobalSettingsEvent)message);
            }
            return NextDelegate.InvokeNextIncoming(message);
        }

        public override Task HandleOutgoing(JObject message)
        {
            return NextDelegate.InvokeNextOutgoing(message);
        }
    }
}
