using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Events
{
    /// <summary>
    /// Values in both Multi and Single action payloads
    /// </summary>
    public interface IActionPayload
    {
        public ActionController Controller { get; set; }
        public bool IsInMultiAction { get; set; }
        public Dictionary<string, string> Resources { get; set; }
        public JObject Settings { get; set; }
        public int State { get; set; }
    }
}
