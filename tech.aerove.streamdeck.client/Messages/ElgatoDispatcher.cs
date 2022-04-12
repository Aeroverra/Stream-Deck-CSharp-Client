using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Messages
{
    public class ElgatoDispatcher
    {
        private readonly WebSocketService _socket;
        public ElgatoDispatcher(WebSocketService socket)
        {
            _socket = socket;
        }
        public void SetSettings(string context, JObject settings)
        {
            var message = new
            {
                Event = "setSettings",
                Context = context,
                Payload = settings
            };
            Task.Run(() => _socket.SendAsync(message)).Wait();
        }
        public Task SetSettingsAsync(string context, JObject settings)
        {
            var message = new
            {
                Event = "setSettings",
                Context = context,
                Payload = settings
            };
            return _socket.SendAsync(message);
        }
        public void ShowOk(string context)
        {
            var message = new
            {
                Event = "showOk",
                Context = context,
            };
            Task.Run(() => _socket.SendAsync(message)).Wait();
        }
        public Task ShowOkAsync(string context)
        {
            var message = new
            {
                Event = "showOk",
                Context = context,
            };
            return _socket.SendAsync(message);
        }
    }
}
