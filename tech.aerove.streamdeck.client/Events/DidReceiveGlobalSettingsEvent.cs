using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Events
{
    public class DidReceiveGlobalSettingsEvent : ElgatoEvent
    {
        public override ElgatoEventType Event { get; set; }
        public JObject Payload { get; set; }
    }
}
