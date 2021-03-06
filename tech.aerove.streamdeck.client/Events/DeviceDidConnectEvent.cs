using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tech.Aerove.StreamDeck.Client.Events.SharedModels;

namespace Tech.Aerove.StreamDeck.Client.Events
{
    public class DeviceDidConnectEvent : ElgatoEvent
    {
        public override ElgatoEventType Event { get; set; }
        public string Device { get; set; }
        public DeviceInfo DeviceInfo { get; set; }
    }
}
