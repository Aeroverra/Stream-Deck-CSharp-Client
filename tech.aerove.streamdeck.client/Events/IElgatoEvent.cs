using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Events
{
    /// <summary>
    /// Covers all ElgatoEvents
    /// </summary>
    public interface IElgatoEvent
    {
        /// <summary>
        /// Received Event Type
        /// </summary>
        ElgatoEventType Event { get; set; }

        /// <summary>
        /// The raw event text received from the Stream Deck
        /// </summary>
        string Raw { get; set; }
        JObject RawJObject { get; set; }
    }
}
