using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tech.Aerove.StreamDeck.Client.Actions
{
    //todo: add image helpers
    public class DefaultActionDispatcher : IActionDispatcher
    {
        private readonly IActionContext _context;
        private readonly IElgatoDispatcher _dispatcher;
        public DefaultActionDispatcher(IElgatoDispatcher elgatoDispatcher, IActionContext actionContext)
        {
            _dispatcher = elgatoDispatcher;
            _context = actionContext;
        }

        public void SetSettings(object settings)
        {
            Task.Run(() => SetSettingsAsync(settings)).Wait();
        }

        public async Task SetSettingsAsync(object settings)
        {
            await _dispatcher.SetSettingsAsync(_context.InstanceId, settings);
            await _dispatcher.GetSettingsAsync(_context.InstanceId);
        }


        public void GetSettings()
        {

            Task.Run(() => GetSettingsAsync()).Wait();
        }

        public Task GetSettingsAsync()
        {
            return _dispatcher.GetSettingsAsync(_context.InstanceId);
        }

        public void SetGlobalSettings(object settings)
        {

            Task.Run(() => SetGlobalSettingsAsync(settings)).Wait();
        }

        public async Task SetGlobalSettingsAsync(object settings)
        {
            await _dispatcher.SetGlobalSettingsAsync(settings);
            await _dispatcher.GetGlobalSettingsAsync();
        }

        public void GetGlobalSettings()
        {

            Task.Run(() => GetGlobalSettingsAsync()).Wait();
        }

        public Task GetGlobalSettingsAsync()
        {
            return _dispatcher.GetGlobalSettingsAsync();
        }

        public void OpenUrl(string url)
        {
            Task.Run(() => OpenUrlAsync(url)).Wait();
        }

        public Task OpenUrlAsync(string url)
        {
            return _dispatcher.OpenUrlAsync(url);
        }

        public void LogMessage(string message)
        {
            Task.Run(() => LogMessageAsync(message)).Wait();
        }

        public Task LogMessageAsync(string message)
        {
            return _dispatcher.LogMessageAsync(message);
        }

        public void SetTitle(string title, int target = 0, int? state = null)
        {

            Task.Run(() => SetTitleAsync(title, target, state)).Wait();
        }

        public Task SetTitleAsync(string title, int target = 0, int? state = null)
        {
            return _dispatcher.SetTitleAsync(_context.InstanceId, title, target, state);
        }

        public void SetImage(string image, int target = 0, int? state = null)
        {

            Task.Run(() => SetImageAsync(image, target, state)).Wait();
        }

        public Task SetImageAsync(string image, int target = 0, int? state = null)
        {
            return _dispatcher.SetImageAsync(_context.InstanceId, image, target, state);
        }

        public void ShowAlert()
        {

            Task.Run(() => ShowAlertAsync()).Wait();
        }

        public Task ShowAlertAsync()
        {
            return _dispatcher.ShowAlertAsync(_context.InstanceId);
        }

        public void ShowOk()
        {

            Task.Run(() => ShowOkAsync()).Wait();
        }

        public Task ShowOkAsync()
        {
            return _dispatcher.ShowOkAsync(_context.InstanceId);
        }

        public void SetState(int state)
        {

            Task.Run(() => SetStateAsync(state)).Wait();
        }

        public Task SetStateAsync(int state)
        {
            return _dispatcher.SetStateAsync(_context.InstanceId, state);
        }

        public void SwitchToProfile(string device, string profile)
        {

            Task.Run(() => SwitchToProfileAsync(device, profile)).Wait();
        }

        public Task SwitchToProfileAsync(string device, string profile)
        {
            return _dispatcher.SwitchToProfileAsync(device, profile);
        }

        public void SendToPropertyInspector(object data)
        {

            Task.Run(() => SendToPropertyInspectorAsync(data)).Wait();
        }

        public Task SendToPropertyInspectorAsync(object data)
        {
            return _dispatcher.SendToPropertyInspectorAsync(_context.InstanceId, _context.ActionUUID, data);
        }

    }
}
