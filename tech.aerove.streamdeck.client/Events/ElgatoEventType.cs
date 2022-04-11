using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Events
{
    public enum ElgatoEventType
    {
        DidReceiveSettings,
        DidReceiveGlobalSettings,
        KeyDown,
        KeyUp,
        WillAppear,
        WillDisappear,
        TitleParametersDidChange,
        DeviceDidConnect,
        DeviceDidDisconnect,
        ApplicationDidLaunch,
        ApplicationDidTerminate,
        SystemDidWakeUp,
        PropertyInspectorDidAppear,
        PropertyInspectorDidDisappear,
        SendToPlugin,
    }
}
