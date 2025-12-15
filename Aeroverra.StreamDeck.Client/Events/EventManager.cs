using Aeroverra.StreamDeck.Client.Events.SDKEvents;
using Microsoft.Extensions.Logging;
using System;

namespace Aeroverra.StreamDeck.Client.Events
{

    /// <summary>
    /// This class provides a way to subscribe to events outside of inheriting
    /// from the actionbase class. Simply inject this as a service
    /// </summary>
    public class EventManager
    {
        private readonly ILogger<EventManager> _logger;
        public EventManager(ILogger<EventManager> logger)
        {
            _logger = logger;
        }
        public event EventHandler<DidReceiveSettingsEvent>? OnDidReceiveSettings;
        public event EventHandler<DidReceiveGlobalSettingsEvent>? OnDidReceiveGlobalSettings;
        public event EventHandler<KeyDownEvent>? OnKeyDown;
        public event EventHandler<KeyUpEvent>? OnKeyUp;
        public event EventHandler<WillAppearEvent>? OnWillAppear;
        public event EventHandler<WillDisappearEvent>? OnWillDisappear;
        public event EventHandler<TitleParametersDidChangeEvent>? OnTitleParametersDidChange;
        public event EventHandler<DeviceDidConnectEvent>? OnDeviceDidConnect;
        public event EventHandler<DeviceDidDisconnectEvent>? OnDeviceDidDisconnect;
        public event EventHandler<ApplicationDidLaunchEvent>? OnApplicationDidLaunch;
        public event EventHandler<ApplicationDidTerminateEvent>? OnApplicationDidTerminate;
        public event EventHandler<SystemDidWakeUpEvent>? OnSystemDidWakeUp;
        public event EventHandler<PropertyInspectorDidAppearEvent>? OnPropertyInspectorDidAppear;
        public event EventHandler<PropertyInspectorDidDisappearEvent>? OnPropertyInspectorDidDisappear;
        public event EventHandler<SendToPluginEvent>? OnSendToPlugin;
        public event EventHandler<DialRotateEvent>? OnDialRotate;
        public event EventHandler<DialDownEvent>? OnDialDown;
        public event EventHandler<DialUpEvent>? OnDialUp;
        public event EventHandler<TouchTapEvent>? OnTouchTap;
        public event EventHandler<OnInitializedEvent>? OnInitialized;
        public event EventHandler<DisposeEvent>? Dispose;


        internal void HandleIncoming(IElgatoEvent? elgatoEvent)
        {

            if (elgatoEvent == null) { return; }

            try
            {
                var actionEvent = elgatoEvent;
                switch (elgatoEvent.Event)
                {
                    case ElgatoEventType.DidReceiveSettings:
                        {
                            var e = (DidReceiveSettingsEvent)actionEvent;
                            OnDidReceiveSettings?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.DidReceiveGlobalSettings:
                        {
                            var e = (DidReceiveGlobalSettingsEvent)actionEvent;
                            OnDidReceiveGlobalSettings?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.KeyDown:
                        {
                            var e = (KeyDownEvent)actionEvent;
                            OnKeyDown?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.KeyUp:
                        {
                            var e = (KeyUpEvent)actionEvent;
                            OnKeyUp?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.WillAppear:
                        {
                            var e = (WillAppearEvent)actionEvent;
                            OnWillAppear?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.WillDisappear:
                        {
                            var e = (WillDisappearEvent)actionEvent;
                            OnWillDisappear?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.TitleParametersDidChange:
                        {
                            var e = (TitleParametersDidChangeEvent)actionEvent;
                            OnTitleParametersDidChange?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.DeviceDidConnect:
                        {
                            var e = (DeviceDidConnectEvent)actionEvent;
                            OnDeviceDidConnect?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.DeviceDidDisconnect:
                        {
                            var e = (DeviceDidDisconnectEvent)actionEvent;
                            OnDeviceDidDisconnect?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.ApplicationDidLaunch:
                        {
                            var e = (ApplicationDidLaunchEvent)actionEvent;
                            OnApplicationDidLaunch?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.ApplicationDidTerminate:
                        {
                            var e = (ApplicationDidTerminateEvent)actionEvent;
                            OnApplicationDidTerminate?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.SystemDidWakeUp:
                        {
                            var e = (SystemDidWakeUpEvent)actionEvent;
                            OnSystemDidWakeUp?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.PropertyInspectorDidAppear:
                        {
                            var e = (PropertyInspectorDidAppearEvent)actionEvent;
                            OnPropertyInspectorDidAppear?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.PropertyInspectorDidDisappear:
                        {
                            var e = (PropertyInspectorDidDisappearEvent)actionEvent;
                            OnPropertyInspectorDidDisappear?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.SendToPlugin:
                        {
                            var e = (SendToPluginEvent)actionEvent;
                            OnSendToPlugin?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.DialRotate:
                        {
                            var e = (DialRotateEvent)actionEvent;
                            OnDialRotate?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.DialDown:
                        {
                            var e = (DialDownEvent)actionEvent;
                            OnDialDown?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.DialUp:
                        {
                            var e = (DialUpEvent)actionEvent;
                            OnDialUp?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.TouchTap:
                        {
                            var e = (TouchTapEvent)actionEvent;
                            OnTouchTap?.Invoke(this, e);
                            break;
                        }
                    case ElgatoEventType.OnInitialized:
                        {
                            var e = (OnInitializedEvent)actionEvent;
                            OnInitialized?.Invoke(this, e);
                            break;
      
                        }
                    case ElgatoEventType.Dispose:
                        {
                            var e = (DisposeEvent)actionEvent;
                            Dispose?.Invoke(this, e);
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled error in event subscriber.");
            }
        }
    }
}
