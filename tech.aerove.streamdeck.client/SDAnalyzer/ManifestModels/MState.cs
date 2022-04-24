using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.SDAnalyzer.ManifestModels
{
    internal class MState
    {
        //Manifest Values
        public string FFamily { get; set; }
        public string FSize { get; set; }
        public string FStyle { get; set; }
        public string FUnderline { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string TitleAlignment { get; set; }
        public string TitleColor { get; set; }
        public string TitleShow { get; set; }

        //Custom Values
        public int Index { get; set; }
        public MAction Action { get; set; }
        public string ImageData { get; set; }
        public FileInfo ImageFile { get; set; }
        public ImageSource ImageSource { get; set; } = ImageSource.Unknown;

        public bool SetImageFromPluginManifests(List<ManifestInfo> pluginManifests)
        {
            var pluginManifest = pluginManifests
                  .Where(x => x.Actions.Any(x => x.Uuid == Action.Uuid))
                  .FirstOrDefault();
            if (pluginManifest == null) { return false; }

            var pluginAction = pluginManifest.Actions.FirstOrDefault(x => x.Uuid == Action.Uuid);
            if(pluginAction.States == null || pluginAction.States.Count< Index + 1) { return false; }

            var pluginStateFile = pluginAction.States[Index].ImageFile;
            if(pluginStateFile == null) { return false; }
            ImageFile = pluginStateFile;
            ImageSource = ImageSource.PluginManifest;
            return true;

        }
        public void Setup(MAction parent, int index, List<ManifestInfo> pluginManifests)
        {
            Index = index;
            Action = parent;
            if (!String.IsNullOrWhiteSpace(Image))
            {
                var path = Path.Combine(Action.Profile.Directory.FullName, $"{Action.Col},{Action.Row}/CustomImages/{Image}");
                var file = new FileInfo(path);
                if (file.Exists)
                {
                    ImageFile = file;
                    ImageSource = ImageSource.User;
                }
            }
            else
            {
                SetImageFromPluginManifests(pluginManifests);
            }
        }
    }
}
