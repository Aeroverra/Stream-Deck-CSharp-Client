using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Events.SharedModels
{
    public class Payload
    {
        public Coordinates Coordinates { get; set; }
        public bool IsInMultiAction { get; set; }
        public JObject Settings { get; set; }
        public int State { get; set; }
        public int UserDesiredState { get; set; }
        public string Title { get; set; }
        public Titleparameters Titleparameters { get; set; }
        public string Application { get; set; }

    }
}
