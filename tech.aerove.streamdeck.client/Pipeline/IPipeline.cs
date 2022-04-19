using tech.aerove.streamdeck.client.Events;

namespace tech.aerove.streamdeck.client.Pipeline
{
    public interface IPipeline
    {
        void SetWebSocket(WebSocketService socket);
        Task HandleIncoming(string message);
        Task HandleOutgoing(object message);
    }
}