﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Events
{
    internal class SystemDidWakeUpEvent : ElgatoEvent
    {
        public override ElgatoEventType Event { get; set; }
    }
}
