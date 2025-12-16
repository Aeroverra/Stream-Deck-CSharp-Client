using Aeroverra.StreamDeck.Client.Events.SDKEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Events
{
    public abstract class ElgatoEvent : IElgatoEvent
    {
        /// <summary>
        /// Event type identifier.
        /// </summary>
        public abstract ElgatoEventType Event { get; set; }

        public  string Raw { get { return _Raw; } set { if (_Raw == "{}") { _Raw = value; } } }
        private string _Raw { get; set; } = "{}";

        public  JObject RawJObject { get { if (_RawJObject == null) { return new JObject(); } return (_RawJObject.DeepClone() as JObject)!; } set { if (_RawJObject == null) { _RawJObject = value; } } }
        private JObject? _RawJObject { get; set; } = null;
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
                case ElgatoEventType.DialRotate:
                    elgatoEvent = JsonConvert.DeserializeObject<DialRotateEvent>(json);
                    break;
                case ElgatoEventType.DialDown:
                    elgatoEvent = JsonConvert.DeserializeObject<DialDownEvent>(json);
                    break;
                case ElgatoEventType.DialUp:
                    elgatoEvent = JsonConvert.DeserializeObject<DialUpEvent>(json);
                    break;
                case ElgatoEventType.TouchTap:
                    elgatoEvent = JsonConvert.DeserializeObject<TouchTapEvent>(json);
                    break;
                case ElgatoEventType.OnInitialized:
                    elgatoEvent = JsonConvert.DeserializeObject<OnInitializedEvent>(json);
                    break;
                case ElgatoEventType.Dispose:
                    elgatoEvent = JsonConvert.DeserializeObject<DisposeEvent>(json);
                    break;
            }
            elgatoEvent.Raw = json;
            elgatoEvent.RawJObject = jsonObject;
            return elgatoEvent;
        }
    }
}
