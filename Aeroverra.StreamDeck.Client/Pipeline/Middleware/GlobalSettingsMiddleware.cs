using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Aeroverra.StreamDeck.Client.Events;

namespace Aeroverra.StreamDeck.Client.Pipeline.Middleware
{
    internal class GlobalSettingsMiddleware : MiddlewareBase
    {
        private readonly ILogger<EventLoggingMiddleware> _logger;
        private readonly IPipeline _pipeline;

        public GlobalSettingsMiddleware(ILogger<EventLoggingMiddleware> logger, IPipeline pipeline)
        {
            _logger = logger;
            _pipeline = pipeline;
        }
        public override Task HandleIncoming(IElgatoEvent message)
        {

            return NextDelegate.InvokeNextIncoming(message);
        }

        public override async Task HandleOutgoing(JObject message)
        {
            if (message["event"].ToString() == "setGlobalSettings")
            {
                var jo = new JObject();
                jo["event"] = "didReceiveGlobalSettings";
                var jo2 = new JObject();
                jo.Add("payload", jo2);
                jo2.Add("settings", message["payload"]);
                await _pipeline.HandleIncoming(jo.ToString());
            }
            await NextDelegate.InvokeNextOutgoing(message);
        }
    }
}
