using tech.aerove.streamdeck.client.Events;

namespace tech.aerove.streamdeck.client.Pipeline.Middleware
{
    public abstract class MiddlewareBase
    {
        public NextDelegate NextDelegate { get; set; }
        public abstract Task HandleIncoming(IElgatoEvent message);
        public abstract Task HandleOutgoing(object message);
    }
}