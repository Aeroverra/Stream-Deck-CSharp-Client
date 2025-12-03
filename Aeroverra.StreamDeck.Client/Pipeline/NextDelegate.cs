using Newtonsoft.Json.Linq;
using Aeroverra.StreamDeck.Client.Events;

namespace Aeroverra.StreamDeck.Client.Pipeline
{
    public class NextDelegate
    {
        public Func<IElgatoEvent, Task> InvokeNextIncoming { get; private set; }
        public Func<JObject, Task> InvokeNextOutgoing { get; private set; }
        internal NextDelegate(Func<IElgatoEvent, Task> nextIncoming, Func<JObject, Task> nextOutgoing)
        {
            InvokeNextIncoming = nextIncoming;
            InvokeNextOutgoing = nextOutgoing;
        }
    }
}
