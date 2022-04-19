using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Events;

namespace tech.aerove.streamdeck.client.Pipeline
{
    public class NextDelegate
    {
        public Func<IElgatoEvent, Task> InvokeNextIncoming { get; private set; }
        public Func<object, Task> InvokeNextOutgoing { get; private set; }
        internal NextDelegate(Func<IElgatoEvent, Task> nextIncoming, Func<object, Task> nextOutgoing)
        {
            InvokeNextIncoming = nextIncoming;
            InvokeNextOutgoing = nextOutgoing;
        }
    }
}
