using Microsoft.Extensions.Logging;
using Aeroverra.StreamDeck.Client.Events;

namespace Aeroverra.StreamDeck.Client.Pipeline
{
    internal class MessageParser
    {
        private readonly ILogger<MessageParser> _logger;
        public MessageParser(ILogger<MessageParser> logger)
        {
            _logger = logger;
        }
        public IElgatoEvent? Parse(string message)
        {
            var elgatoEvent = ElgatoEvent.FromJson(message);
            if (elgatoEvent == null)
            {
                _logger.LogWarning("Could not parse incoming message {incomingmessage}.", message);
            }
            return elgatoEvent;
        }
    }
}
