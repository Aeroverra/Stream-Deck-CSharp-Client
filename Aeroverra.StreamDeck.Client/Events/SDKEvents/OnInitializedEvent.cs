using Aeroverra.StreamDeck.Client.Events.SharedModels;

namespace Aeroverra.StreamDeck.Client.Events.SDKEvents
{
    /// <summary>
    /// Called the first time the action is initialized
    /// </summary>
    public class OnInitializedEvent : ElgatoEvent, IActionEvent, ISDKEvent
    {
        public string Action { get; set; } = null!;
        public string Context { get; set; } = null!;
        public required string Device { get; set; }
        public override ElgatoEventType Event { get; set; }
        public required Payload Payload { get; set; }
    }
}
