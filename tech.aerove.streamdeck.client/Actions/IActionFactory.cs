using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Events;

namespace tech.aerove.streamdeck.client.Actions
{
    public interface IActionFactory
    {
        List<ActionBase> CreateActions(ElgatoEvent elgatoEvent);
    }
}
