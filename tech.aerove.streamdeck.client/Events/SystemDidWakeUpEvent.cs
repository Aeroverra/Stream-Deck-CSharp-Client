using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tech.Aerove.StreamDeck.Client.Events
{
    public class SystemDidWakeUpEvent : ElgatoEvent
    {
        public override ElgatoEventType Event { get; set; }
    }
}
