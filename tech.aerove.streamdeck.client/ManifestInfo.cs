using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client
{
    public class ManifestInfo
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Category { get; set; } = "Custom";
        public string CodePath { get; set; }
        public string? CodePathWin { get; set; }
        public string? CodePathMac { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public List<ManifestAction> Actions { get; set; } = new List<ManifestAction>();
        public List<ManifestProfile> Profiles { get; set; } = new List<ManifestProfile>();
        public ManifestInfo()
        {
            string json = System.IO.File.ReadAllText("manifest.json");
            JObject jo = JObject.Parse(json);
            Name = $"{jo["Name"]}";
            Author = $"{jo["Author"]}";
            Category = $"{jo["Category"] ?? Category}";
            CodePath = $"{jo["CodePath"]}";
            CodePathWin = $"{jo["CodePathWin"] ?? null}";
            CodePathMac = $"{jo["CodePathMac"] ?? null}";
            Description = $"{jo["Description"]}";
            Version = $"{jo["Version"]}";
            Actions = JsonConvert.DeserializeObject<List<ManifestAction>>($"{jo["Actions"]}");
            if (jo["Profiles"] != null)
            {
                Profiles = JsonConvert.DeserializeObject<List<ManifestProfile>>($"{jo["Profiles"]}");
            }
        }
    }
    public class ManifestAction
    {
        public string Icon { get; set; } = "";
        public string Name { get; set; } = "";

        public List<ManifestActionState> States { get; set; } = new List<ManifestActionState>();

        public bool? VisibleInActionsList { get; set; }
        public bool? SupportedInMultiActions { get; set; }
        public string? Tooltip { get; set; }
        public string Uuid { get; set; } = "";
        public string? PropertyInspectorPath { get; set; }
        public bool? DisableCaching { get; set; }
    }
    public class ManifestActionState
    {

        public string Image { get; set; } = "";
        public string? Name { get; set; }
        public bool? OnlyForMultiAction { get; set; }
    }
    public class ManifestProfile
    {
        public string Name { get; set; } = "";
        public bool ReadOnly { get; set; } = false;
        public long DeviceType { get; set; }
        public bool DontAutoSwitchWhenInstalled { get; set; } = false;
    }
}
