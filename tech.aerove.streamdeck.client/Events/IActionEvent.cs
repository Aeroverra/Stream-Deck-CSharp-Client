using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Events
{
    public interface IActionEvent
    {
        string Action { get; set; }
        string Context { get; set; }
    }
}
