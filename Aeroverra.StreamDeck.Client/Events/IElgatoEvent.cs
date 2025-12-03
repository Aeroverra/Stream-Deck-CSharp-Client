using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Events
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
