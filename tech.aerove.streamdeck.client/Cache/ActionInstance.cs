using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Cache
{
    public class ActionInstance
    {
        public string Id { get; set; }
        public string UUID { get; set; }
        public int? Column { get; set; }
        public int? Row { get; set; }
        public bool IsInMultiAction { get; set; }
        public JObject Settings { get; set; }
        public int State { get; set; }
        public bool IsShown { get; set; }
        public Device Device { get; set; }

        //title stuff
        public string Title { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public string FontStyle { get; set; }
        public bool FontUnderline { get; set; } = false;
        public bool ShowTitle { get; set; } = true;
        public string TitleAlignment { get; set; }
        public string TitleColor { get; set; }


    }
}
