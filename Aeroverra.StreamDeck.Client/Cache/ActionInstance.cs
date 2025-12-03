using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Cache
{
    public class ActionInstance
    {
        /// <summary>
        /// A value to identify the instance's action. You will need to pass this opaque value to several APIs like the setTitle API.
        /// also known as the context for this instance of an action
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A value to identify the instance's action. You will need to pass this opaque value to several APIs like the setTitle API.
        /// Also known as the manifest uuid for this action
        /// </summary>
        public string UUID { get; set; }
        public int? Column { get; set; }
        public int? Row { get; set; }
        public bool IsInMultiAction { get; set; }
        public JObject Settings { get; set; }
        public int State { get; set; }
        public bool IsShown { get; set; }
        public Device Device { get; set; }

        //title stuff
        public string Title { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public string FontStyle { get; set; }
        public bool FontUnderline { get; set; } = false;
        public bool ShowTitle { get; set; } = true;
        public string TitleAlignment { get; set; }
        public string TitleColor { get; set; }


    }
}
