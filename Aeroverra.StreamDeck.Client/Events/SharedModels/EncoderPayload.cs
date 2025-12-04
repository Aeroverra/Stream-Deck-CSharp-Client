using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Aeroverra.StreamDeck.Client.Events.SharedModels
{
    public class EncoderPayload
    {
        public string Controller { get; set; }
        public Coordinates Coordinates { get; set; }
        public Dictionary<string, string> Resources { get; set; }
        public JObject Settings { get; set; }
    }

    public class DialRotatePayload : EncoderPayload
    {
        public bool Pressed { get; set; }
        public int Ticks { get; set; }
    }

    public class TouchTapPayload : EncoderPayload
    {
        public bool Hold { get; set; }
        public int[] TapPos { get; set; }
    }
}
