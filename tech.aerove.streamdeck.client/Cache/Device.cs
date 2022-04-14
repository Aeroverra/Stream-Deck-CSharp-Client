using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Cache
{
    public class Device
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public int Type { get; set; }
        public bool IsConnected { get; set; }   

        public List<ActionInstance> ActionInstances { get; set; } = new List<ActionInstance>();
    }
}
