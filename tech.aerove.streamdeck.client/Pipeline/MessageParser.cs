using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tech.Aerove.StreamDeck.Client.Events;

namespace Tech.Aerove.StreamDeck.Client.Pipeline
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
