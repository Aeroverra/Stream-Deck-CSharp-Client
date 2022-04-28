using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tech.Aerove.StreamDeck.Client.Events
{
    public abstract class ElgatoEvent : IElgatoEvent
    {
        public abstract ElgatoEventType Event { get; set; }

        public virtual string Raw { get { return _Raw; } set { if (_Raw == "") { _Raw = value; } } } 
        private string _Raw { get; set; } = "";

        public virtual JObject RawJObject { get { return _RawJObject.DeepClone() as JObject; } set { if (_RawJObject == null) { _RawJObject = value; } } }
        public virtual JObject _RawJObject { get; set; }
        public static ElgatoEvent? FromJson(string json)
        {
            JObject jsonObject = JObject.Parse(json);

            string eventName = jsonObject["event"].ToString();
            ElgatoEventType EventType = (ElgatoEventType)Enum.Parse(typeof(ElgatoEventType), eventName, true);
            ElgatoEvent elgatoEvent = null;
            switch (EventType)
            {
                case ElgatoEventType.DidReceiveSettings:
                    elgatoEvent = JsonConvert.DeserializeObject<DidReceiveSettingsEvent>(json);
                    break;
                case ElgatoEventType.DidReceiveGlobalSettings:
                    elgatoEvent = JsonConvert.DeserializeObject<DidReceiveGlobalSettingsEvent>(json);
                    break;
                case ElgatoEventType.KeyDown:
                    elgatoEvent = JsonConvert.DeserializeObject<KeyDownEvent>(json);
                    break;
                case ElgatoEventType.KeyUp:
                    elgatoEvent = JsonConvert.DeserializeObject<KeyUpEvent>(json);
                    break;
                case ElgatoEventType.WillAppear:
                    elgatoEvent = JsonConvert.DeserializeObject<WillAppearEvent>(json);
                    break;
                case ElgatoEventType.WillDisappear:
                    elgatoEvent = JsonConvert.DeserializeObject<WillDisappearEvent>(json);
                    break;
                case ElgatoEventType.TitleParametersDidChange:
                    elgatoEvent = JsonConvert.DeserializeObject<TitleParametersDidChangeEvent>(json);
                    break;
                case ElgatoEventType.DeviceDidConnect:
                    elgatoEvent = JsonConvert.DeserializeObject<DeviceDidConnectEvent>(json);
                    break;
                case ElgatoEventType.DeviceDidDisconnect:
                    elgatoEvent = JsonConvert.DeserializeObject<DeviceDidDisconnectEvent>(json);
                    break;
                case ElgatoEventType.ApplicationDidLaunch:
                    elgatoEvent = JsonConvert.DeserializeObject<ApplicationDidLaunchEvent>(json);
                    break;
                case ElgatoEventType.ApplicationDidTerminate:
                    elgatoEvent = JsonConvert.DeserializeObject<ApplicationDidTerminateEvent>(json);
                    break;
                case ElgatoEventType.SystemDidWakeUp:
                    elgatoEvent = JsonConvert.DeserializeObject<SystemDidWakeUpEvent>(json);
                    break;
                case ElgatoEventType.PropertyInspectorDidAppear:
                    elgatoEvent = JsonConvert.DeserializeObject<PropertyInspectorDidAppearEvent>(json);
                    break;
                case ElgatoEventType.PropertyInspectorDidDisappear:
                    elgatoEvent = JsonConvert.DeserializeObject<PropertyInspectorDidDisappearEvent>(json);
                    break;
                case ElgatoEventType.SendToPlugin:
                    elgatoEvent = JsonConvert.DeserializeObject<SendToPluginEvent>(json);
                    break;
            }
            elgatoEvent.Raw = json;
            elgatoEvent.RawJObject = jsonObject;
            return elgatoEvent;
        }
    }
}
