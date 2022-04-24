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
        public DirectoryInfo DirectoryInfo { get; set; }
        public List<ManifestAction> Actions { get; set; } = new List<ManifestAction>();
        public List<ManifestProfile> Profiles { get; set; } = new List<ManifestProfile>();

        public ManifestInfo() : this(new FileInfo("manifest.json"))
        {

        }

        public ManifestInfo(FileInfo file)
        {
            DirectoryInfo = file.Directory;
            string json = System.IO.File.ReadAllText(file.FullName);
            JObject jo = JObject.Parse(json);
            Name = $"{jo["Name"]}";
            Author = $"{jo["Author"]}";
            Category = $"{jo["Category"] ?? Category}";
            CodePath = $"{jo["CodePath"]}";
            CodePathWin = $"{jo["CodePathWin"] ?? null}";
            CodePathMac = $"{jo["CodePathMac"] ?? null}";
            Description = $"{jo["Description"]}";
            Version = $"{jo["Version"]}";
            Actions = JsonConvert.DeserializeObject<List<ManifestAction>>($"{jo["Actions"]}") ?? Actions;
            if (jo["Profiles"] != null)
            {
                Profiles = JsonConvert.DeserializeObject<List<ManifestProfile>>($"{jo["Profiles"]}");
            }
        }
        public void Setup()
        {
            foreach (var action in Actions)
            {
                action.Setup(this);
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

        //custom
        public ManifestInfo? ManifestInfo { get; set; }
        public void Setup(ManifestInfo parent)
        {
            ManifestInfo = parent;
            var index = 0;
            foreach (var state in States)
            {
                state.Setup(this, index++);
            }
        }
    }
    public class ManifestActionState
    {

        public string Image { get; set; } = "";
        public string? Name { get; set; }
        public bool? OnlyForMultiAction { get; set; }


        //custom
        int? Index { get; set; }
        public ManifestAction? ManifestAction { get; set; }
        public FileInfo? ImageFile { get; set; }
        public void Setup(ManifestAction parent, int index)
        {
            Index = index;
            ManifestAction = parent;
            if (String.IsNullOrWhiteSpace(Image))
            {
                return;
            }
            var imagesPath = Path.Combine(ManifestAction.ManifestInfo.DirectoryInfo.FullName, Image);
            var imagesDir = new DirectoryInfo(imagesPath).Parent;
            if (imagesDir == null || !imagesDir.Exists)
            {
                return;
            }
            //get image name without any paths. This is done this way because we don't know the extension
            var imageName = new FileInfo(Image).Name;
            var file = imagesDir.GetFiles().FirstOrDefault(x => x.Name.StartsWith(imageName));
            if (file == null || !file.Exists)
            {
                return;
            }
            ImageFile = file;
        }
    }
    public class ManifestProfile
    {
        public string Name { get; set; } = "";
        public bool ReadOnly { get; set; } = false;
        public long DeviceType { get; set; }
        public bool DontAutoSwitchWhenInstalled { get; set; } = false;
    }
}
