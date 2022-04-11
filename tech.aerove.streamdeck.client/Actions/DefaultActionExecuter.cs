using Microsoft.Extensions.Logging;
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
                switch (elgatoEvent.Event)
                {
                    case ElgatoEventType.DidReceiveSettings:
                        action.DidReceiveSettings();
                        await action.DidReceiveSettingsAsync();
                        break;
                    case ElgatoEventType.DidReceiveGlobalSettings:
                        action.DidReceiveGlobalSettings();
                        await action.DidReceiveGlobalSettingsAsync();
                        break;
                    case ElgatoEventType.KeyDown:
                        action.KeyDown();
                        await action.KeyDownAsync();
                        break;
                    case ElgatoEventType.KeyUp:
                        action.KeyUp();
                        await action.KeyUpAsync();
                        break;
                    case ElgatoEventType.WillAppear:
                        action.WillAppear();
                        await action.WillAppearAsync();
                        break;
                    case ElgatoEventType.WillDisappear:
                        action.WillDisappear();
                        await action.WillDisappearAsync();
                        break;
                    case ElgatoEventType.TitleParametersDidChange:
                        action.TitleParametersDidChange();
                        await action.TitleParametersDidChangeAsync();
                        break;
                    case ElgatoEventType.DeviceDidConnect:
                        action.DeviceDidConnect();
                        await action.DeviceDidConnectAsync();
                        break;
                    case ElgatoEventType.DeviceDidDisconnect:
                        action.DeviceDidDisconnect();
                        await action.DeviceDidDisconnectAsync();
                        break;
                    case ElgatoEventType.ApplicationDidLaunch:
                        action.ApplicationDidLaunch();
                        await action.ApplicationDidLaunchAsync();
                        break;
                    case ElgatoEventType.ApplicationDidTerminate:
                        action.ApplicationDidTerminate();
                        await action.ApplicationDidTerminateAsync();
                        break;
                    case ElgatoEventType.SystemDidWakeUp:
                        action.SystemDidWakeUp();
                        await action.SystemDidWakeUpAsync();
                        break;
                    case ElgatoEventType.PropertyInspectorDidAppear:
                        action.PropertyInspectorDidAppear();
                        await action.PropertyInspectorDidAppearAsync();
                        break;
                    case ElgatoEventType.PropertyInspectorDidDisappear:
                        action.PropertyInspectorDidDisappear();
                        await action.PropertyInspectorDidDisappearAsync();
                        break;
                    case ElgatoEventType.SendToPlugin:
                        action.SendToPlugin();
                        await action.SendToPluginAsync();
                        break;
                }
            }
        }
    }
}
