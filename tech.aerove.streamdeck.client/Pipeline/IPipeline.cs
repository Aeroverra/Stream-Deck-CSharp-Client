using tech.aerove.streamdeck.client.Events;

namespace tech.aerove.streamdeck.client.Pipeline
{
    public interface IPipeline
    {
        Task HandleIncoming(string message);
        Task HandleOutgoing(object message);
    }
}