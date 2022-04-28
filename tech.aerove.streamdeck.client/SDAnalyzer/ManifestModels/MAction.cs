using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tech.Aerove.StreamDeck.Client.SDAnalyzer.ManifestModels
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
        public MProfile Profile { get; set; }

        public int Col { get; set; }
        public int Row { get; set; }
        public bool IsMultiAction = false;

        //used by child actions when Main Action is in MultiAction
        public bool IsRoutine { get; set; } = false;
        public bool IsRoutineAlt { get; set; } = false;

        //used when Main Action is MultiAction
        [JsonProperty("Routine")]
        public List<MAction> Actions { get; set; } = new List<MAction>();
        //used for multiaction parent
        public MAction Action { get; set; }


        public void Setup(MProfile parent, List<ManifestInfo> pluginManifests)
        {
            Profile = parent;

            //covers routine and routine2 aka multiactions
            if (Uuid.Contains("com.elgato.streamdeck.multiactions.routine"))
            {
               IsMultiAction = true;
                var actions = (Settings["Routine"] as JArray).ToObject<List<MAction>>();
                var actionAlts = (Settings["RoutineAlt"] as JArray).ToObject<List<MAction>>();
                actions.ForEach(x => x.IsRoutine = true);
                actionAlts.ForEach(x => x.IsRoutineAlt = true);
                Actions.AddRange(actions);
                Actions.AddRange(actionAlts);
                Actions.ForEach(x =>
                {
                    x.Col = Col;
                    x.Row = Row;
                });
            }
            //setup states
            var index = 0;
            foreach (var state in States)
            {
                state.Setup(this,index++, pluginManifests);
            }
            foreach(var action in Actions)
            {
                action.Setup(this, pluginManifests);
            }
        }
        //setup for inner multiactions
        public void Setup(MAction parent, List<ManifestInfo> pluginManifests)
        {
            Action = parent;

            //setup states
            var index = 0;
            foreach (var state in States)
            {
                state.Setup(this, index++, pluginManifests);
            }
        }
    }
}
