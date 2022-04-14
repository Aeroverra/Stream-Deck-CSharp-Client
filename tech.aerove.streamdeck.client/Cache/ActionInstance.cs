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
        public string UUID { get;set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public bool IsInMultiAction { get; set; }
        public JObject Settings { get; set; }
        public int State { get; set; }
        public string Title { get; set; }
    }
}
