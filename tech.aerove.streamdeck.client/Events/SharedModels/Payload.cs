using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tech.Aerove.StreamDeck.Client.Events.SharedModels
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
