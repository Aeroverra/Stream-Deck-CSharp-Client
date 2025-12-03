using Aeroverra.StreamDeck.Client.Events;

namespace Aeroverra.StreamDeck.Client.Actions
{
    public interface IActionExecuter
    {
        /// <summary>
        /// executes a list of actions
        /// </summary>
        /// <param name="elgatoEvent"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        Task ExecuteAsync(IElgatoEvent elgatoEvent, List<ActionBase> actions);
    }
}
