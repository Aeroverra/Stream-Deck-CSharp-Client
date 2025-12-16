using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aeroverra.StreamDeck.Client.Events.SharedModels
{
    public class TouchTapPayload : EncoderPayload
    {
        public bool Hold { get; set; }
        public int[] TapPos { get; set; }
    }
}
