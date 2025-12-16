using Aeroverra.StreamDeck.Client.Events.SharedModels;
using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Events
{
    public class SendToPluginEvent : ElgatoEvent, IActionEvent
    {
        public Guid SDKId { get; set; }
        public override ElgatoEventType Event { get; set; }
        public string Action { get; set; }
        public string Context { get; set; }
        public JObject payload { get; set; }

        /// <summary>
        /// Will always be null due to unfixed bug with stream deck firmware
        /// </summary>
        public string Device { get; set; } = null!;
    }
}
