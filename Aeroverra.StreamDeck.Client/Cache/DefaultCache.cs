using Aeroverra.StreamDeck.Client.Actions;
using Aeroverra.StreamDeck.Client.Events;
using Aeroverra.StreamDeck.Client.Events.SDKEvents;
using Aeroverra.StreamDeck.Client.Events.SharedModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Cache
{
    internal class DefaultCache() : ICache
    {
        public List<Device> Devices = new List<Device>();
        public JObject GlobalSettings = new JObject();
        public List<MonitoredApplication> MonitoredApplications = new List<MonitoredApplication>();

        public void Update(IElgatoEvent e)
        {
            lock (Devices)
            {
                switch (e.Event)
                {
                    case ElgatoEventType.DidReceiveSettings:
                        DidReceiveSettings((DidReceiveSettingsEvent)e);
                        return;
                    case ElgatoEventType.DidReceiveGlobalSettings:
                        DidReceiveGlobalSettings((DidReceiveGlobalSettingsEvent)e);
                        return;
                    case ElgatoEventType.KeyDown:
                        KeyDown((KeyDownEvent)e);
                        return;
                    case ElgatoEventType.KeyUp:
                        KeyUp((KeyUpEvent)e);
                        return;
                    case ElgatoEventType.WillAppear:
                        WillAppear((WillAppearEvent)e);
                        return;
                    case ElgatoEventType.WillDisappear:
                        WillDisappear((WillDisappearEvent)e);
                        return;
                    case ElgatoEventType.TitleParametersDidChange:
                        TitleParametersDidChange((TitleParametersDidChangeEvent)e);
                        return;
                    case ElgatoEventType.DeviceDidConnect:
                        DeviceDidConnect((DeviceDidConnectEvent)e);
                        return;
                    case ElgatoEventType.DeviceDidDisconnect:
                        DeviceDidDisconnect((DeviceDidDisconnectEvent)e);
                        return;
                    case ElgatoEventType.ApplicationDidLaunch:
                        ApplicationDidLaunch((ApplicationDidLaunchEvent)e);
                        return;
                    case ElgatoEventType.ApplicationDidTerminate:
                        ApplicationDidTerminate((ApplicationDidTerminateEvent)e);
                        return;
                    case ElgatoEventType.DialRotate:
                        DialRotate((DialRotateEvent)e);
                        return;
                    case ElgatoEventType.DialDown:
                        DialDown((DialDownEvent)e);
                        return;
                    case ElgatoEventType.DialUp:
                        DialUp((DialUpEvent)e);
                        return;
                    case ElgatoEventType.TouchTap:
                        TouchTap((TouchTapEvent)e);
                        return;

                    // SDK Events
                    case ElgatoEventType.OnInitialized:
                        OnInitialized((OnInitializedEvent)e);
                        return;
                    case ElgatoEventType.Dispose:
                        Dispose((DisposeEvent)e);
                        return;
                }
            }
        }

        public IActionContext BuildContext(string instanceId)
        {
            var instance = Devices
                     .SelectMany(x => x.ActionInstances)
                     .Single(x => x.Id == instanceId);

            return new DefaultActionContext(this, instance);
        }

        private void DidReceiveSettings(DidReceiveSettingsEvent e)
        {
            var device = Devices.Single(x => x.Id == e.Device);
            var instance = device.ActionInstances.Single(x => x.SDKId == e.SDKId);
            instance.Column = e.Payload.Coordinates.Column;
            instance.Row = e.Payload.Coordinates.Row;
            instance.Settings = e.Payload.Settings;
            instance.IsInMultiAction = e.Payload.IsInMultiAction;
        }

        private void DidReceiveGlobalSettings(DidReceiveGlobalSettingsEvent e)
        {
            GlobalSettings = e.Payload.Settings;
        }

        private void KeyDown(KeyDownEvent e)
        {
            var device = Devices.Single(x => x.Id == e.Device);
            var instance = device.ActionInstances.Single(x => x.SDKId == e.SDKId);
            instance.Column = e.Payload.Coordinates.Column;
            instance.Row = e.Payload.Coordinates.Row;
            instance.Settings = e.Payload.Settings;
            instance.IsInMultiAction = e.Payload.IsInMultiAction;
            instance.State = e.Payload.State;
        }

        private void KeyUp(KeyUpEvent e)
        {
            var device = Devices.Single(x => x.Id == e.Device);
            var instance = device.ActionInstances.Single(x => x.SDKId == e.SDKId);
            instance.Column = e.Payload.Coordinates.Column;
            instance.Row = e.Payload.Coordinates.Row;
            instance.Settings = e.Payload.Settings;
            instance.IsInMultiAction = e.Payload.IsInMultiAction;
            instance.State = e.Payload.State;
        }

        private void DialRotate(DialRotateEvent e)
        {
            UpdateEncoderInstance(e.Device, e.SDKId, e.Payload);
        }

        private void DialDown(DialDownEvent e)
        {
            UpdateEncoderInstance(e.Device, e.SDKId, e.Payload);
        }

        private void DialUp(DialUpEvent e)
        {
            UpdateEncoderInstance(e.Device, e.SDKId, e.Payload);
        }

        private void TouchTap(TouchTapEvent e)
        {
            UpdateEncoderInstance(e.Device, e.SDKId, e.Payload);
        }

        private void UpdateEncoderInstance(string deviceId, Guid sdkId, EncoderPayload payload)
        {
            var device = Devices.Single(x => x.Id == deviceId);
            var instance = device.ActionInstances.Single(x => x.SDKId == sdkId);
            instance.Column = payload.Coordinates?.Column;
            instance.Row = payload.Coordinates?.Row;
            instance.Settings = payload.Settings;
        }

        // Creates a new Instance when moving but if you switch views and back it is the same instance
        private void WillAppear(WillAppearEvent e)
        {
            var device = Devices.Single(x => x.Id == e.Device);
            var instance = device.ActionInstances.Single(x => x.SDKId == e.SDKId);
            instance.Id = e.Context;
            instance.Settings = e.Payload.Settings;
            instance.Column = e.Payload.Coordinates?.Column;
            instance.Row = e.Payload.Coordinates?.Row;
            instance.State = e.Payload.State;
            instance.IsInMultiAction = e.Payload.IsInMultiAction;
            instance.IsShown = true;

        }

        private void WillDisappear(WillDisappearEvent e)
        {
            var device = Devices.Single(x => x.Id == e.Device);
            var instance = device.ActionInstances.Single(x => x.SDKId == e.SDKId);
            instance.Settings = e.Payload.Settings;
            instance.Column = e.Payload.Coordinates?.Column;
            instance.Row = e.Payload.Coordinates?.Row;
            instance.State = e.Payload.State;
            instance.IsInMultiAction = e.Payload.IsInMultiAction;
            instance.IsShown = false;
        }

        private void TitleParametersDidChange(TitleParametersDidChangeEvent e)
        {
            var device = Devices.Single(x => x.Id == e.Device);
            var instance = device.ActionInstances.Single(x => x.SDKId == e.SDKId);
            instance.Column = e.payload.Coordinates.Column;
            instance.Row = e.payload.Coordinates.Row;
            instance.Settings = e.payload.Settings;
            instance.State = e.payload.State;
            instance.Title = e.payload.Title;
            instance.FontFamily = e.payload.Titleparameters.fontFamily;
            instance.FontSize = e.payload.Titleparameters.fontSize;
            instance.FontStyle = e.payload.Titleparameters.fontStyle;
            instance.FontUnderline = e.payload.Titleparameters.fontUnderline;
            instance.ShowTitle = e.payload.Titleparameters.showTitle;
            instance.TitleAlignment = e.payload.Titleparameters.titleAlignment;
            instance.TitleColor = e.payload.Titleparameters.titleColor;

        }

        private void DeviceDidConnect(DeviceDidConnectEvent e)
        {
            var device = Devices.SingleOrDefault(x => x.Id == e.Device);
            if (device == null)
            {
                device = new Device();
                Devices.Add(device);
            }
            device.Id = e.Device;
            device.Name = e.DeviceInfo.Name;
            device.Columns = e.DeviceInfo.Size.Columns;
            device.Rows = e.DeviceInfo.Size.Rows;
            device.Type = e.DeviceInfo.Type;
            device.IsConnected = true;
        }

        private void DeviceDidDisconnect(DeviceDidDisconnectEvent e)
        {
            var device = Devices.Single(x => x.Id == e.Device);
            device.IsConnected = false;
        }

        private void ApplicationDidLaunch(ApplicationDidLaunchEvent e)
        {
            var application = MonitoredApplications.SingleOrDefault(x => x.Name == e.Payload.Application);
            if (application == null)
            {
                application = new MonitoredApplication
                {
                    Name = e.Payload.Application,
                    IsRunning = true
                };
                MonitoredApplications.Add(application);
            }
            application.IsRunning = true;
        }

        private void ApplicationDidTerminate(ApplicationDidTerminateEvent e)
        {
            var application = MonitoredApplications.Single(x => x.Name == e.Payload.Application);
            application.IsRunning = false;
        }

        private void OnInitialized(OnInitializedEvent e)
        {
            var device = Devices.SingleOrDefault(x => x.Id == e.Device);
            //This needs to be here to handle when devices are not connected
            if (device == null)
            {
                device = new Device
                {
                    Id = e.Device,
                    Name = "UNKNOWN",
                    Columns = -1,
                    Rows = -1,
                    Type = -1,
                    IsConnected = false
                };
                Devices.Add(device);
            }
            var instance = device.ActionInstances.SingleOrDefault(x => x.SDKId == e.SDKId);
            if (instance == null)
            {
                instance = new ActionInstance
                {
                    SDKId = e.SDKId,
                    Id = e.Context,
                    UUID = e.Action,
                    Column = e.Payload.Coordinates?.Column,
                    Row = e.Payload.Coordinates?.Row,
                    IsInMultiAction = e.Payload.IsInMultiAction,
                    State = e.Payload.State,
                    Settings = e.Payload.Settings,
                    Title = e.Payload.Title,
                    Device = device,
                    IsShown = true
                };
                device.ActionInstances.Add(instance);
            }
            instance.Settings = e.Payload.Settings;
            instance.Column = e.Payload.Coordinates?.Column;
            instance.Row = e.Payload.Coordinates?.Row;
            instance.State = e.Payload.State;
            instance.IsInMultiAction = e.Payload.IsInMultiAction;
            instance.IsShown = true;

        }

        private void Dispose(DisposeEvent e)
        {
            var device = Devices.Single(x => x.Id == e.Device);
            var instance = device.ActionInstances.Single(x => x.SDKId == e.SDKId);
            device.ActionInstances.Remove(instance);
            if (device.ActionInstances.Count == 0)
            {
                Devices.Remove(device);
            }

        }


        public List<string> GetAllInstanceIds()
        {
            return Devices
                .SelectMany(x => x.ActionInstances)
                .Select(x => x.Id)
                .ToList();
        }

    }
}
