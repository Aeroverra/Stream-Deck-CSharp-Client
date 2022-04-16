using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Cache;

namespace tech.aerove.streamdeck.client.Actions
{
    internal class DefaultActionContext : IActionContext
    {
        public DefaultActionContext(JObject globalSettings, ActionInstance instance)
        {
            DeviceId = instance.Device.Id;
            InstanceId = instance.Id;
            ActionUUID = instance.UUID;
            Column = instance.Column;
            Row = instance.Row;
            IsInMultiAction = instance.IsInMultiAction;
            State = instance.State;
            IsShown = instance.IsShown;
            DeviceIsConnected = instance.Device.IsConnected;
            Title = instance.Title;
            FontFamily = instance.FontFamily;
            FontSize = instance.FontSize;
            FontStyle = instance.FontStyle;
            FontUnderline = instance.FontUnderline;
            ShowTitle = instance.ShowTitle;
            TitleAlignment = instance.TitleAlignment;
            TitleColor  = instance.TitleColor;
            var settings = instance.Settings.DeepClone();
            Settings = (JObject)settings;
            var globalSettings2 = globalSettings.DeepClone();
            GlobalSettings = (JObject)globalSettings2;
        }
        public string DeviceId {get; private set;}

        public string InstanceId { get; set; }

        public string ActionUUID {get; private set;}

        public int? Column {get; private set;}

        public int? Row {get; private set;}

        public bool IsInMultiAction {get; private set;}

        public int State {get; private set;}

        public bool IsShown {get; private set;}

        public bool DeviceIsConnected {get; private set;}

        public string Title {get; private set;}

        public string FontFamily {get; private set;}

        public int FontSize {get; private set;}

        public string FontStyle {get; private set;}

        public bool FontUnderline {get; private set;}

        public bool ShowTitle {get; private set;}

        public string TitleAlignment {get; private set;}

        public string TitleColor {get; private set;}

        public JObject Settings {get; private set;}

        public JObject GlobalSettings {get; private set;}
    }
}
