using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tech.Aerove.StreamDeck.Client.Events
{
    [JsonConverter(typeof(StringEnumConverter))]
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
