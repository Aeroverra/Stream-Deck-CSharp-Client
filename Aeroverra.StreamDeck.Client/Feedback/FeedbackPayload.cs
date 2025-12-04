using System.Collections.Generic;

namespace Aeroverra.StreamDeck.Client.Feedback
{
    /// <summary>
    /// Represents the payload object for setFeedback, keyed by control id.
    /// </summary>
    public class FeedbackPayload : Dictionary<string, FeedbackValue>
    {
    }
}
