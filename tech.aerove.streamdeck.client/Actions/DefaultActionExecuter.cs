using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Events;

namespace tech.aerove.streamdeck.client.Actions
{
    internal class DefaultActionExecuter : IActionExecuter
    {
        private readonly ILogger<DefaultActionExecuter> _logger;
        public DefaultActionExecuter(ILogger<DefaultActionExecuter> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(ElgatoEvent elgatoEvent, List<ActionBase> actions)
        {

            foreach (ActionBase action in actions)
            {
                //todo: find a way to deepcopy or prevent changing vars
                var actionEvent = elgatoEvent;
                switch (elgatoEvent.Event)
                {
                    case ElgatoEventType.DidReceiveSettings:
                        {
                            var e = (DidReceiveSettingsEvent)actionEvent;
                            action.DidReceiveSettings(e);
                            await action.DidReceiveSettingsAsync(e);
                            break;
                        }
                    case ElgatoEventType.DidReceiveGlobalSettings:
                        {
                            var e = (DidReceiveGlobalSettingsEvent)actionEvent;
                            action.DidReceiveGlobalSettings(e);
                            await action.DidReceiveGlobalSettingsAsync(e);
                            break;
                        }
                    case ElgatoEventType.KeyDown:
                        {
                            var e = (KeyDownEvent)actionEvent;
                            action.KeyDown(e);
                            await action.KeyDownAsync(e);
                            break;
                        }
                    case ElgatoEventType.KeyUp:
                        {
                            var e = (KeyUpEvent)actionEvent;
                            action.KeyUp(e);
                            await action.KeyUpAsync(e);
                            break;
                        }
                    case ElgatoEventType.WillAppear:
                        {
                            var e = (WillAppearEvent)actionEvent;
                            action.WillAppear(e);
                            await action.WillAppearAsync(e);
                            break;
                        }
                    case ElgatoEventType.WillDisappear:
                        {
                            var e = (WillDisappearEvent)actionEvent;
                            action.WillDisappear(e);
                            await action.WillDisappearAsync(e);
                            break;
                        }
                    case ElgatoEventType.TitleParametersDidChange:
                        {
                            var e = (TitleParametersDidChangeEvent)actionEvent;
                            action.TitleParametersDidChange(e);
                            await action.TitleParametersDidChangeAsync(e);
                            break;
                        }
                    case ElgatoEventType.DeviceDidConnect:
                        {
                            var e = (DeviceDidConnectEvent)actionEvent;
                            action.DeviceDidConnect(e);
                            await action.DeviceDidConnectAsync(e);
                            break;
                        }
                    case ElgatoEventType.DeviceDidDisconnect:
                        {
                            var e = (DeviceDidDisconnectEvent)actionEvent;
                            action.DeviceDidDisconnect(e);
                            await action.DeviceDidDisconnectAsync(e);
                            break;
                        }
                    case ElgatoEventType.ApplicationDidLaunch:
                        {
                            var e = (ApplicationDidLaunchEvent)actionEvent;
                            action.ApplicationDidLaunch(e);
                            await action.ApplicationDidLaunchAsync(e);
                            break;
                        }
                    case ElgatoEventType.ApplicationDidTerminate:
                        {
                            var e = (ApplicationDidTerminateEvent)actionEvent;
                            action.ApplicationDidTerminate(e);
                            await action.ApplicationDidTerminateAsync(e);
                            break;
                        }
                    case ElgatoEventType.SystemDidWakeUp:
                        {
                            var e = (SystemDidWakeUpEvent)actionEvent;
                            action.SystemDidWakeUp(e);
                            await action.SystemDidWakeUpAsync(e);
                            break;
                        }
                    case ElgatoEventType.PropertyInspectorDidAppear:
                        {
                            var e = (PropertyInspectorDidAppearEvent)actionEvent;
                            action.PropertyInspectorDidAppear(e);
                            await action.PropertyInspectorDidAppearAsync(e);
                            break;
                        }
                    case ElgatoEventType.PropertyInspectorDidDisappear:
                        {
                            var e = (PropertyInspectorDidDisappearEvent)actionEvent;
                            action.PropertyInspectorDidDisappear(e);
                            await action.PropertyInspectorDidDisappearAsync(e);
                            break;
                        }
                    case ElgatoEventType.SendToPlugin:
                        {
                            var e = (SendToPluginEvent)actionEvent;
                            action.SendToPlugin(e);
                            await action.SendToPluginAsync(e);
                            break;
                        }
                }
            }
        }
    }
}
