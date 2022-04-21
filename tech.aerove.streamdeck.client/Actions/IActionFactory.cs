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
        /// <summary>
        /// Returns a list of instances related to this action which should then 
        /// be executed based on the event
        /// </summary>
        /// <param name="elgatoEvent">A Received elgato event</param>
        /// <returns></returns>
        List<ActionBase> CreateActions(IElgatoEvent elgatoEvent);
    }
}
