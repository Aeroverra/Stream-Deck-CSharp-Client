using Aeroverra.StreamDeck.Client.Events.SharedModels;

namespace Aeroverra.StreamDeck.Client.Events.SDKEvents
{
    /// <summary>
    /// When <see cref="IDisposable"/> or <see cref="IAsyncDisposable"/> is implemented this event will call them when the action is removed from the Stream Deck device and the instance is considered dead."/>
    /// </summary>
    public class DisposeEvent : ElgatoEvent, IActionEvent, ISDKEvent
    {
        public Guid SDKId { get; set; }
        public string Action { get; set; } = null!;
        public string Context { get; set; } = null!;
        public required string Device { get; set; }
        public override ElgatoEventType Event { get; set; }
        public required Payload Payload { get; set; }
    }
}
