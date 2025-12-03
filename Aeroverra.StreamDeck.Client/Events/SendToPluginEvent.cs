using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Events
{
    public class SendToPluginEvent : ElgatoEvent, IActionEvent
    {
        public override ElgatoEventType Event { get; set; }
        public string Action { get; set; }
        public string Context { get; set; }
        public JObject payload { get; set; }
    }
}
