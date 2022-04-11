using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Messages
{
    internal class RegistrationEvent : IElgatoMessage
    {
        public string Event { get; set; } = "";
        public string UUID { get; set; } = "";
    }
}
