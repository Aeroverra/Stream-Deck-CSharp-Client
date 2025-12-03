using Tech.Aerove.StreamDeck.Client.Events;

namespace Tech.Aerove.StreamDeck.Client.Actions
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
