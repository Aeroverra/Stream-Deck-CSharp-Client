﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Events.SharedModels;

namespace tech.aerove.streamdeck.client.Events
{
    public class WillAppearEvent : ElgatoEvent, IActionEvent
    {
        public override ElgatoEventType Event { get; set; }
        public string Action { get; set; }
        public string Context { get; set; }
        public string Device { get; set; }
        public Payload Payload { get; set; }

        //added by StreamDeckAnalyzerMiddelware
        public bool? WasCreated { get; set; }

    }
}
