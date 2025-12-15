namespace Aeroverra.StreamDeck.Client.Events
{
    public class PropertyInspectorDidAppearEvent : ElgatoEvent, IActionEvent
    {
        public Guid SDKId { get; set; }
        public override ElgatoEventType Event { get; set; }
        public string Action { get; set; }
        public string Context { get; set; }
        public string Device { get; set; }
    }
}
