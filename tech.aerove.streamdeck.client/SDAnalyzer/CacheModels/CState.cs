using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.SDAnalyzer.SettingsModels
{
    internal class CState
    {
        public string Title { get; set; } = "";
        string ImageData = "";
        ImageSource ImageSource { get; set; } = ImageSource.Unknown;
    }
}
