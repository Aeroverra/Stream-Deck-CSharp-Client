using Tech.Aerove.StreamDeck.Client.Events.SharedModels;

namespace Tech.Aerove.StreamDeck.Client.Events
{
    public class DidReceiveGlobalSettingsEvent : ElgatoEvent
    {
        public override ElgatoEventType Event { get; set; }
        public Payload Payload { get; set; }
    }
}
