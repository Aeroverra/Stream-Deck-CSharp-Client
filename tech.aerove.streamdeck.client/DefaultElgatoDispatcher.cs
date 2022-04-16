﻿using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Startup;

namespace tech.aerove.streamdeck.client
{
    public class DefaultElgatoDispatcher : IElgatoDispatcher
    {
        private WebSocketService _socket
        {
            get
            {
                if (underlyingSocket == null)
                {
                    underlyingSocket = _services.GetService<WebSocketService>();
                }
                return underlyingSocket;
            }
        }
        private WebSocketService? underlyingSocket { get; set; }
        private readonly StreamDeckInfo _streamDeckInfo;
        private readonly IServiceProvider _services;
        public DefaultElgatoDispatcher(IServiceProvider services, StreamDeckInfo streamDeckInfo)
        {
            //serviceprovider to avoid circular refrence
            _services = services;
            _streamDeckInfo = streamDeckInfo;
        }

        public void SendRegisterEvent()
        {
            Task.Run(() => SendRegisterEventAsync()).Wait();
        }

        public Task SendRegisterEventAsync()
        {
            var message = new
            {
                UUID = _streamDeckInfo.PluginUUID,
                Event = _streamDeckInfo.RegisterEvent
            };
            return _socket.SendAsync(message);
        }

        public void SetSettings(string context, object settings)
        {
            Task.Run(() => SetSettingsAsync(context, settings)).Wait();
        }

        public Task SetSettingsAsync(string context, object settings)
        {
            var message = new
            {
                Event = "setSettings",
                Context = context,
                Payload = settings
            };
            return _socket.SendAsync(message);
        }


        public void GetSettings(string context)
        {

            Task.Run(() => GetSettingsAsync(context)).Wait();
        }

        public Task GetSettingsAsync(string context)
        {
            var message = new
            {
                Event = "getSettings",
                Context = context,
            };
            return _socket.SendAsync(message);
        }

        public void SetGlobalSettings(object settings)
        {

            Task.Run(() => SetGlobalSettingsAsync(settings)).Wait();
        }

        public Task SetGlobalSettingsAsync(object settings)
        {
            var message = new
            {
                Event = "setGlobalSettings",
                Context = _streamDeckInfo.PluginUUID,
                Payload = settings
            };
            return _socket.SendAsync(message);
        }

        public void GetGlobalSettings()
        {

            Task.Run(() => GetGlobalSettingsAsync()).Wait();
        }

        public Task GetGlobalSettingsAsync()
        {
            var message = new
            {
                Event = "getGlobalSettings",
                Context = _streamDeckInfo.PluginUUID,
            };
            return _socket.SendAsync(message);
        }

        public void OpenUrl(string url)
        {

            Task.Run(() => OpenUrlAsync(url)).Wait();
        }

        public Task OpenUrlAsync(string url)
        {
            var message = new
            {
                Event = "openUrl",
                Payload = new
                {
                    Url = url
                }
            };
            return _socket.SendAsync(message);
        }

        public void LogMessage(string message)
        {

            Task.Run(() => LogMessageAsync(message)).Wait();
        }

        public Task LogMessageAsync(string message)
        {
            var message2 = new
            {
                Event = "logMessage",
                Payload = new
                {
                    message
                }
            };
            return _socket.SendAsync(message2);
        }

        public void SetTitle(string context, string title, int target = 0, int? state = null)
        {

            Task.Run(() => SetTitleAsync(context, title, target, state)).Wait();
        }

        public Task SetTitleAsync(string context, string title, int target = 0, int? state = null)
        {
            var message = new
            {
                Event = "setTitle",
                Context = context,
                Payload = new
                {
                    Title = title,
                    Target = target,
                    State = state
                }
            };
            return _socket.SendAsync(message);
        }

        public void SetImage(string context, string image, int target = 0, int? state = null)
        {

            Task.Run(() => SetImageAsync(context, image, target, state)).Wait();
        }

        public Task SetImageAsync(string context, string image, int target = 0, int? state = null)
        {
            var message = new
            {
                Event = "setImage",
                Context = context,
                Payload = new
                {
                    Image = image,
                    Target = target,
                    State = state
                }
            };
            return _socket.SendAsync(message);
        }

        public void ShowAlert(string context)
        {

            Task.Run(() => ShowAlertAsync(context)).Wait();
        }

        public Task ShowAlertAsync(string context)
        {
            var message = new
            {
                Event = "showAlert",
                Context = context,
            };
            return _socket.SendAsync(message);
        }

        public void ShowOk(string context)
        {

            Task.Run(() => ShowOkAsync(context)).Wait();
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

        public void SetState(string context, int state)
        {

            Task.Run(() => SetStateAsync(context, state)).Wait();
        }

        public Task SetStateAsync(string context, int state)
        {
            var message = new
            {
                Event = "setState",
                Context = context,
                Payload = new
                {
                    State = state
                }
            };
            return _socket.SendAsync(message);
        }

        public void SwitchToProfile(string device, string profile)
        {

            Task.Run(() => SwitchToProfileAsync(device, profile)).Wait();
        }

        public Task SwitchToProfileAsync(string device, string profile)
        {
            var message = new
            {
                Event = "switchToProfile",
                Context = _streamDeckInfo.PluginUUID,
                Device = device,
                Payload = new
                {
                    Profile = profile
                }
            };
            return _socket.SendAsync(message);
        }

        public void SendToPropertyInspector(string context, string action, object data)
        {

            Task.Run(() => SendToPropertyInspectorAsync(context, action, data)).Wait();
        }

        public Task SendToPropertyInspectorAsync(string context, string action, object data)
        {
            var message = new
            {
                Event = "sendToPropertyInspector",
                Action = action,
                Context = context,
                Payload = data
            };
            return _socket.SendAsync(message);
        }

    }
}