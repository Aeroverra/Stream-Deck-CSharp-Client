using Tech.Aerove.StreamDeck.Client.Events;

namespace Tech.Aerove.StreamDeck.Client.Pipeline
{
    public interface IPipeline
    {
        void SetWebSocket(WebSocketService socket);
        Task HandleIncoming(string message);
        Task HandleOutgoing(object message);
    }
}