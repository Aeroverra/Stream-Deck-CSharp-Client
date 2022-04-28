using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tech.Aerove.StreamDeck.Client.Events;

namespace Tech.Aerove.StreamDeck.Client.Pipeline
{
    public interface IElgatoMiddleware
    {
        Task HandleIncoming(IElgatoEvent message);
        Task HandleOutgoing(object message);
    }
}
