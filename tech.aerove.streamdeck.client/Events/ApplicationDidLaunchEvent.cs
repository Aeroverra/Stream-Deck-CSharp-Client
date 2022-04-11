using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Models;

namespace tech.aerove.streamdeck.client.Events
{
    public class ApplicationDidLaunchEvent : ElgatoEvent
    {
        public override ElgatoEventType Event { get; set; }
        public Payload Payload { get; set; }
    }
}
