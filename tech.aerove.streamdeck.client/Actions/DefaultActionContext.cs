using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Cache;

namespace tech.aerove.streamdeck.client.Actions
{
    internal class DefaultActionContext : IActionContext
    {
        private readonly DefaultCache _cache;
        private readonly ActionInstance _instance;
        public DefaultActionContext(DefaultCache cache, ActionInstance instance)
        {
            _cache = cache;
            _instance = instance;
        }

        public string DeviceId { get { return _instance.Device.Id; } }

        public string InstanceId { get { return _instance.Id; } }

        public string ActionUUID { get { return _instance.UUID; } }

        public int? Column { get { return _instance.Column; } }

        public int? Row { get { return _instance.Row; } }

        public bool IsInMultiAction { get { return _instance.IsInMultiAction; } }

        public int State { get { return _instance.State; } }

        public bool IsShown { get { return _instance.IsShown; } }

        public bool DeviceIsConnected { get { return _instance.Device.IsConnected; } }

        public string Title { get { return _instance.Title; } }

        public string FontFamily { get { return _instance.FontFamily; } }

        public int FontSize { get { return _instance.FontSize; } }

        public string FontStyle { get { return _instance.FontStyle; } }

        public bool FontUnderline { get { return _instance.FontUnderline; } }

        public bool ShowTitle { get { return _instance.ShowTitle; } }

        public string TitleAlignment { get { return _instance.TitleAlignment; } }

        public string TitleColor { get { return _instance.TitleColor; } }

        public JObject Settings
        {
            get
            {
                var settings = _instance.Settings.DeepClone();
                return (JObject)settings;
            }
        }

        public JObject GlobalSettings
        {
            get
            {
                var globalSettings = _cache.GlobalSettings.DeepClone();
                return (JObject)globalSettings;
            }
        }
    }
}
