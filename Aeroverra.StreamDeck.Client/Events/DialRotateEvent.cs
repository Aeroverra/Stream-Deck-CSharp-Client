using Aeroverra.StreamDeck.Client.Events.SharedModels;

namespace Aeroverra.StreamDeck.Client.Events
{
    public class DialRotateEvent : ElgatoEvent, IActionEvent
    {
        /// <inheritdoc />
        public Guid SDKId { get; set; }

        /// <inheritdoc />
        public override ElgatoEventType Event { get; set; }

        /// <inheritdoc />
        public string Action { get; set; }

        /// <inheritdoc />
        public string Context { get; set; }

        /// <summary>
        /// Unique identifier of the Stream Deck device that this event is associated with.
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// Contextualized information for this event.
        /// </summary>
        public DialRotatePayload Payload { get; set; }
    }
}
