namespace Aeroverra.StreamDeck.Client.Pipeline
{
    public interface IPipeline
    {
        Task StartListening(CancellationToken cancellationToken);
        Task HandleIncoming(string message);
        Task HandleOutgoing(object message);
    }
}