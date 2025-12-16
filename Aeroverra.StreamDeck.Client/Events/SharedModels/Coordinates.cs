using System.Text;

namespace Aeroverra.StreamDeck.Client.Events.SharedModels
{
    public class Coordinates
    {
        /// <summary>
        /// Column the action instance is located in, indexed from 0.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Row the action instance is located on, indexed from 0.
        /// Note: When the device is Stream Deck +, the row can be 0 for keys(Keypad), and will always be 0 for dials(Encoder); to differentiate between actions types, cross-check the controller.
        /// </summary>
        public int Row { get; set; }
    }
}
