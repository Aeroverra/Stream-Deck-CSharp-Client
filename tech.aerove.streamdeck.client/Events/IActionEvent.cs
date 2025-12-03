namespace Tech.Aerove.StreamDeck.Client.Events
{
    /// <summary>
    /// This is used when events specify and action instance
    /// </summary>
    public interface IActionEvent
    {
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
