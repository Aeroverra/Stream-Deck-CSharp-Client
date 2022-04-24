using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.SDAnalyzer.ManifestModels
{
    internal class MAction
    {
        //Manifest Values
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Settings")]
        public JObject Settings { get; set; }

        [JsonProperty("State")]
        public int State { get; set; }

        [JsonProperty("States")]
        public List<MState> States { get; set; } = new List<MState>();

        [JsonProperty("UUID")]
        public string Uuid { get; set; }


        //used by child actions when Main Action is in MultiAction
        [JsonProperty("OverrideState")]
        public int OverrideState { get; set; }


        //------Custom Values------
        public int Col { get; set; }
        public int Row { get; set; }
        public bool IsMultiAction = false;

        //used by child actions when Main Action is in MultiAction
        public bool IsRoutine { get; set; } = false;
        public bool IsRoutineAlt { get; set; } = false;

        //used when Main Action is MultiAction
        [JsonProperty("Routine")]
        public List<MAction> Actions { get; set; } = new List<MAction>();
    }
}
