using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.SDAnalyzer.SettingsModels
{
    internal class CProfile
    {
        //the folder name - sdProfile
        public string UUID { get; set; } = "";
        public List<CAction> Actions { get; set; } = new List<CAction>();
    }
}
