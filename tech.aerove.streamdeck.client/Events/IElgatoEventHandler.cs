using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Events
{
    /// <summary>
    /// Handles incoming requests from the StreamDeck
    /// </summary>
    public interface IElgatoEventHandler
    {
        Task<IElgatoEvent?> HandleIncoming(string message);
    }
}
