using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Events.SharedModels
{
    public class EncoderPayload
    {
        /// <summary>
        /// Defines the controller type the action is applicable to. 
        /// Keypad refers to a standard action on a Stream Deck device, e.g. 1 of the 15 buttons on the Stream Deck MK.2, or a pedal on the Stream Deck Pedal, etc., 
        /// whereas an Encoder refers to a dial / touchscreen on the Stream Deck +.
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Coordinates that identify the location of the action.
        /// </summary>
        public Coordinates Coordinates { get; set; }

        /// <summary>
        /// Resources (files) associated with the action.
        /// </summary>
        public Dictionary<string, string> Resources { get; set; }

        /// <summary>
        /// Settings associated with the action instance.
        /// </summary>
        public JObject Settings { get; set; }
    }
}
