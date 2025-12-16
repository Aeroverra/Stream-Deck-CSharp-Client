using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Actions
{
    /// <summary>
    /// All the information related to this actions instance
    /// </summary>
    public interface IActionContext
    {
        /// <summary>
        /// SDK assigned unique identifier for this action instance
        /// </summary>
        public Guid SDKId { get; }

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

        /// <summary>
        /// Boolean indicating if the action is inside a Multi-Action.
        /// </summary>
        public bool IsInMultiAction { get; }

        /// <summary>
        /// The 0-based value contains the current state of the action.
        /// </summary>
        public int State { get; }

        /// <summary>
        /// Indicates whether or not this instance is currently shown on the stream deck
        /// </summary>
        public bool IsShown { get; }

        /// <summary>
        /// Indicates whether or not this instances device is currently connected
        /// </summary>
        public bool DeviceIsConnected { get; }

        /// <summary>
        /// Title displayed on the action
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The font family for the title.
        /// </summary>
        public string FontFamily { get; }

        /// <summary>
        /// The font size for the title.
        /// </summary>
        public int FontSize { get; }

        /// <summary>
        /// The font style for the title.
        /// </summary>
        public string FontStyle { get; }

        /// <summary>
        /// Boolean indicating an underline under the title.
        /// </summary>
        public bool FontUnderline { get; }

        /// <summary>
        /// Boolean indicating if the title is visible.
        /// </summary>
        public bool ShowTitle { get; }

        /// <summary>
        /// Vertical alignment of the title. Possible values are "top", "bottom" and "middle".
        /// </summary>
        public string TitleAlignment { get; }

        /// <summary>
        /// Title color.
        /// </summary>
        public string TitleColor { get; }

        /// <summary>
        /// This JSON object contains persistently stored data for this instance of an action.
        /// </summary>
        public JObject Settings { get; }

        /// <summary>
        /// This JSON object contains persistently stored data for this plugin
        /// </summary>
        public JObject GlobalSettings { get; }
    }
}
