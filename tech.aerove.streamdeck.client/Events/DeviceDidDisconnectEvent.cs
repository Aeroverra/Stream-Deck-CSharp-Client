using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tech.Aerove.StreamDeck.Client.Events
{
    public class DeviceDidDisconnectEvent : ElgatoEvent
    {
        public override ElgatoEventType Event { get; set; }
        public string Device { get; set; }
    }
}
