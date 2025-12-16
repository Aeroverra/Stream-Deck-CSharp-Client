using Aeroverra.StreamDeck.Client.Events.SharedModels;

namespace Aeroverra.StreamDeck.Client.Events.SDKEvents
{
    public sealed class DialStopEvent : ElgatoEvent, IActionEvent, ISDKEvent
    {
        /// <inheritdoc />
        public Guid SDKId { get; set; }

        /// <inheritdoc />
        public override ElgatoEventType Event { get; set; }

        /// <inheritdoc />
        public string Action { get; set; }

        /// <inheritdoc />
        public string Context { get; set; }

        /// <inheritdoc />
        public string Device { get; set; }

        /// <summary>
        /// Contextualized information for this event.
        /// </summary>
        public DialRotatePayload Payload { get; set; }

        /// <summary>
        /// Duration in milliseconds when the dial stopped turning and the event was fired
        /// </summary>
        public int MSDelay { get; set; }

        /// <summary>
        /// Length of the dial stop event.
        /// </summary>
        public DialStopEventLength EventLength { get; set; }
    }
}
