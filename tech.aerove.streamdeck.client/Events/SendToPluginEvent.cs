using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tech.Aerove.StreamDeck.Client.Events
{
    public class SendToPluginEvent : ElgatoEvent, IActionEvent
    {
        public override ElgatoEventType Event { get; set; }
        public string Action { get; set; }
        public string Context { get; set; }
        public JObject payload { get; set; }
    }
}
