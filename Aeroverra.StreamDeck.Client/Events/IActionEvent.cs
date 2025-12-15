using Aeroverra.StreamDeck.Client.Events.SharedModels;

namespace Aeroverra.StreamDeck.Client.Events
{
    /// <summary>
    /// This is used when events specify and action instance
    /// </summary>
    public interface IActionEvent
    {
        /// <summary>
        /// An Id assigned by the SDK to identify your action
        /// Used to track an instance when its moved to prevent reinitialization and even between sessions
        /// </summary>
        public Guid SDKId { get; set; }

        /// <summary>
        /// The action's unique identifier. If your plugin supports multiple actions, you should use this value to find out which action was triggered.
        /// </summary>
        string Action { get; set; }

        /// <summary>
        /// A value to identify the instance's action. You will need to pass this opaque value to several APIs like the setTitle API.
        /// </summary>
        string Context { get; set; }
    }
}
