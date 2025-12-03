using Tech.Aerove.StreamDeck.Client.Events;

namespace Tech.Aerove.StreamDeck.Client.Pipeline
{
    public interface IElgatoMiddleware
    {
        Task HandleIncoming(IElgatoEvent message);
        Task HandleOutgoing(object message);
    }
}
