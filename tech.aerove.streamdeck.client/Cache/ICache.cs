﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Actions;
using tech.aerove.streamdeck.client.Events;

namespace tech.aerove.streamdeck.client.Cache
{
    /// <summary>
    /// Handles storing and updating data recieved from the StreamDeck to be shared with 
    /// other portions of the client
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Updates the cache data with new event info
        /// </summary>
        /// <param name="e">The event recieved from Elgato</param>
        void Update(IElgatoEvent e);

        /// <summary>
        /// Builds an action context for the specified instance for use when handling events
        /// </summary>
        /// <param name="instanceId">Context or instanceid associated with this instace of 
        /// an action.</param>
        /// <returns>Action Context</returns>
        IActionContext BuildContext(string instanceId);

        /// <summary>
        /// Returns all known instance id's
        /// </summary>
        /// <returns></returns>
        List<string> GetAllInstanceIds();
    }
}
