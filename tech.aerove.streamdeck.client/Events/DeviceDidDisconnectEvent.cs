namespace Tech.Aerove.StreamDeck.Client.Events
{
    public class DeviceDidDisconnectEvent : ElgatoEvent
    {
        public override ElgatoEventType Event { get; set; }
        public string Device { get; set; }
    }
}
