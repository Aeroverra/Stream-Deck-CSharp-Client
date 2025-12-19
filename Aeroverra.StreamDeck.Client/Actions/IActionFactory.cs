using Aeroverra.StreamDeck.Client.Events;

namespace Aeroverra.StreamDeck.Client.Actions
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

	/// <summary>
	/// Allows the factory to perform post-execution maintenance such as disposing
	/// of owned scopes for actions that have completed their lifetime (e.g. dispose events).
	/// </summary>
	/// <param name="elgatoEvent">The event that was executed.</param>
	/// <param name="actions">The actions that were executed for this event.</param>
	ValueTask CleanupAsync(IElgatoEvent elgatoEvent, IReadOnlyCollection<ActionBase> actions);
}
}
