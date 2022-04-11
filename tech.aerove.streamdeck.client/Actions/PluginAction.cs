using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Actions
{

    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginAction : Attribute
    {

        public string Id { get; private set; }

        public PluginAction(string ActionId)
        {
            Id = ActionId;
        }
    }
}
