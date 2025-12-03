using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.SDAnalyzer.ManifestModels
{
    /// <summary>
    /// Represents a profile manifest
    /// Children are folders and pages
    /// </summary>
    internal class MProfile
    {

        //Manifest Values
        public string DeviceModel { get; set; }
        public string DeviceUuid { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public List<MAction> Actions { get; set; } = new List<MAction>();


        //Custom Values
        public string UUID { get; set; } //the folder name - sdProfile
        public bool IsTopLevel { get; set; } = false; //in the profilesv2 folder
        public bool IsFolder { get; set; } = false;

        public bool IsPage { get; set; } = false;
        public bool IsFirstPage { get; set; } = false;
        public MProfile NextPage { get; set; }
        public MProfile PreviousPage { get; set; }

        public DirectoryInfo Directory;
        public MProfile Parent { get; set; }
        public List<MProfile> Children { get; set; } = new List<MProfile>();



        internal MProfile(string json, DirectoryInfo directory, List<ManifestInfo> pluginManifests, MProfile? parent = null)
        {
            Directory = directory;
            UUID = directory.Name.Replace(".sdProfile", "");
            if (parent != null)
            {
                Parent = parent;
                parent.Children.Add(this);
            }
            var jo = JObject.Parse(json);
            DeviceModel = $"{jo["DeviceModel"]}";
            DeviceUuid = $"{jo["DeviceUUID"]}";
            Name = $"{jo["Name"]}";
            Version = $"{jo["Version"]}";
            var actionsJson = jo["Actions"];
            foreach (var actionJson in actionsJson as JObject)
            {
                var action = actionJson.Value.ToObject<MAction>();
                var colrow = actionJson.Key.Split(",");
                action.Col = int.Parse(colrow[0]);
                action.Row = int.Parse(colrow[1]);
                Actions.Add(action);
            }
            if (parent == null)
            {
                IsTopLevel = true;
            }
            var nextButton = Actions.FirstOrDefault(x => x.Uuid == "com.elgato.streamdeck.page.next");
            var previousButton = Actions.FirstOrDefault(x => x.Uuid == "com.elgato.streamdeck.page.previous");
            if (nextButton != null || previousButton != null)
            {
                IsPage = true;
            }
            //will always be at 0,0
            var backButton = Actions.FirstOrDefault(x => x.Uuid == "com.elgato.streamdeck.profile.backtoparent");
            if (backButton != null)
            {
                IsFolder = true;
            }

            foreach (var action in Actions)
            {
                action.Setup(this, pluginManifests);
            }
        }
    }
}
