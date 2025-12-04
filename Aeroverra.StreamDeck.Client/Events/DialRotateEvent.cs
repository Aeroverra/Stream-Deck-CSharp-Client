using Aeroverra.StreamDeck.Client.Events.SharedModels;

namespace Aeroverra.StreamDeck.Client.Events
{
    public class DialRotateEvent : ElgatoEvent, IActionEvent
    {
        public override ElgatoEventType Event { get; set; }
        public string Action { get; set; }
        public string Context { get; set; }
        public string Device { get; set; }
        public DialRotatePayload Payload { get; set; }
    }
}
