using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Events
{
    public abstract class ElgatoEvent : IElgatoEvent
    {
        public abstract ElgatoEventType Event { get; set; }

        public static ElgatoEvent? FromJson(string json)
        {
            JObject jsonObject = JObject.Parse(json);

            string eventName = jsonObject["event"].ToString();
            ElgatoEventType EventType = (ElgatoEventType)Enum.Parse(typeof(ElgatoEventType), eventName, true);
            switch (EventType)
            {
                case ElgatoEventType.DidReceiveSettings:
                    return JsonConvert.DeserializeObject<DidReceiveSettingsEvent>(json);
                case ElgatoEventType.DidReceiveGlobalSettings:
                    return JsonConvert.DeserializeObject<DidReceiveGlobalSettingsEvent>(json);
                case ElgatoEventType.KeyDown:
                    return JsonConvert.DeserializeObject<KeyDownEvent>(json);
                case ElgatoEventType.KeyUp:
                    return JsonConvert.DeserializeObject<KeyUpEvent>(json);
                case ElgatoEventType.WillAppear:
                    return JsonConvert.DeserializeObject<WillAppearEvent>(json);
                case ElgatoEventType.WillDisappear:
                    return JsonConvert.DeserializeObject<WillDisappearEvent>(json);
                case ElgatoEventType.TitleParametersDidChange:
                    return JsonConvert.DeserializeObject<TitleParametersDidChangeEvent>(json);
                case ElgatoEventType.DeviceDidConnect:
                    return JsonConvert.DeserializeObject<DeviceDidConnectEvent>(json);
                case ElgatoEventType.DeviceDidDisconnect:
                    return JsonConvert.DeserializeObject<DeviceDidDisconnectEvent>(json);
                case ElgatoEventType.ApplicationDidLaunch:
                    return JsonConvert.DeserializeObject<ApplicationDidLaunchEvent>(json);
                case ElgatoEventType.ApplicationDidTerminate:
                    return JsonConvert.DeserializeObject<ApplicationDidTerminateEvent>(json);
                case ElgatoEventType.SystemDidWakeUp:
                    return JsonConvert.DeserializeObject<SystemDidWakeUpEvent>(json);
                case ElgatoEventType.PropertyInspectorDidAppear:
                    return JsonConvert.DeserializeObject<PropertyInspectorDidAppearEvent>(json);
                case ElgatoEventType.PropertyInspectorDidDisappear:
                    return JsonConvert.DeserializeObject<PropertyInspectorDidDisappearEvent>(json);
                case ElgatoEventType.SendToPlugin:
                    return JsonConvert.DeserializeObject<SendToPluginEvent>(json);
            }
            return null;
        }
    }
}
