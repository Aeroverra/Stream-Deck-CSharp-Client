using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Actions
{
    /// <summary>
    /// All the information related to this actions instance
    /// </summary>
    public interface IActionContext
    {
        /// <summary>
        /// The Device this instance is on
        /// </summary>
        public string DeviceId { get; }

        /// <summary>
        /// The context identifier of the instance.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        /// The uuid of the action specified in the manifest
        /// </summary>
        public string ActionUUID { get; }

        /// <summary>
        /// How many columns on this device
        /// </summary>
        public int? Column { get; }

        /// <summary>
        /// How many rows on this device
        /// </summary>
        public int? Row { get; }


        public bool IsInMultiAction { get; }
        public int State { get; }
        public bool IsShown { get; }
        public bool DeviceIsConnected { get; }
        public string Title { get; }
        public string FontFamily { get; }
        public int FontSize { get; }
        public string FontStyle { get; }
        public bool FontUnderline { get; }
        public bool ShowTitle { get; }
        public string TitleAlignment { get; }
        public string TitleColor { get; }
        public JObject Settings { get; }
        public JObject GlobalSettings { get; }
    }
}
