using Newtonsoft.Json.Linq;
using Aeroverra.StreamDeck.Client.Events;

namespace Aeroverra.StreamDeck.Client.Pipeline.Middleware
{
    public abstract class MiddlewareBase
    {
        public NextDelegate NextDelegate { get; set; }
        public abstract Task HandleIncoming(IElgatoEvent message);
        public abstract Task HandleOutgoing(JObject message);
    }
}