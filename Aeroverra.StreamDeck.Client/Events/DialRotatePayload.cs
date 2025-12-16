using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aeroverra.StreamDeck.Client.Events.SharedModels
{
    public class DialRotatePayload : EncoderPayload
    {
        /// <summary>
        /// Determines whether the dial was pressed whilst the rotation occurred.
        /// </summary>
        public bool Pressed { get; set; }

        /// <summary>
        /// Number of ticks the dial was rotated; this can be a positive (clockwise) or negative (counter-clockwise) number.
        /// </summary>
        public int Ticks { get; set; }
    }
}
