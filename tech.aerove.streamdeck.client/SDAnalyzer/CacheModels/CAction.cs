using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.SDAnalyzer.SettingsModels
{
    internal class CAction
    {
        public string Name { get; set; } = "";
        public int Col { get; set; }
        public int Row { get; set; }
        public int State { get; set; }
        public List<CState> States { get; set; } = new List<CState>();
    }
}
