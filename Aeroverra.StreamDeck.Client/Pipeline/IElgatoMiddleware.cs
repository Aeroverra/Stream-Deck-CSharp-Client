using Aeroverra.StreamDeck.Client.Events;

namespace Aeroverra.StreamDeck.Client.Pipeline
{
    public interface IElgatoMiddleware
    {
        Task HandleIncoming(IElgatoEvent message);
        Task HandleOutgoing(object message);
    }
}
