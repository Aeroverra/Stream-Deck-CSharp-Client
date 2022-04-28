using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tech.Aerove.StreamDeck.Client.Events.SharedModels;

namespace Tech.Aerove.StreamDeck.Client.Events
{
    public class ApplicationDidTerminateEvent : ElgatoEvent
    {
        public override ElgatoEventType Event { get; set; }
        public Payload Payload { get; set; }
    }
}
