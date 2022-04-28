using Newtonsoft.Json.Linq;
using Tech.Aerove.StreamDeck.Client.Events;

namespace Tech.Aerove.StreamDeck.Client.Pipeline.Middleware
{
    public abstract class MiddlewareBase
    {
        public NextDelegate NextDelegate { get; set; }
        public abstract Task HandleIncoming(IElgatoEvent message);
        public abstract Task HandleOutgoing(JObject message);
    }
}