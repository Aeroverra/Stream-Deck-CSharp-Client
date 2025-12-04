namespace Aeroverra.StreamDeck.Client
{
    public interface IElgatoWebSocket
    {
        public Task ConnectAsync();
        Task DisconnectAsync();
        Task<string?> ListenAsync();
        Task SendAsync(object message);
    }
}
