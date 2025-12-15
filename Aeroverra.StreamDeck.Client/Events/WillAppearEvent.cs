using Aeroverra.StreamDeck.Client.Events.SharedModels;

namespace Aeroverra.StreamDeck.Client.Events
{
    public class WillAppearEvent : ElgatoEvent, IActionEvent
    {
        public Guid SDKId { get; set; }
        public override ElgatoEventType Event { get; set; }
        public string Action { get; set; }
        public string Context { get; set; }
        public string Device { get; set; }
        public Payload Payload { get; set; }

        //added by StreamDeckAnalyzerMiddelware
        public bool? WasCreated { get; set; }
    }
}
