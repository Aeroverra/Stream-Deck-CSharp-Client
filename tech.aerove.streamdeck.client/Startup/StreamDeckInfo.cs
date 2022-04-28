using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tech.Aerove.StreamDeck.Client.Startup
{
    public class StreamDeckInfo
    {

        public int Port { get; set; }
        public string PluginUUID { get; set; } = "";
        public string RegisterEvent { get; set; } = "";
        public DeviceInfo Info { get; set; } = new DeviceInfo();

        //Example Startup Args
        //-port
        //28196
        //-pluginUUID
        //4B435A96A4390157D43EF7C6EFBBF7DD
        //-registerEvent
        //registerPlugin
        //-info
        //{"application":{"font":"MS Shell Dlg 2","language":"en","platform":"windows","platformVersion":"10.0.19043","version":"5.2.1.15025"},"colors":{"buttonMouseOverBackgroundColor":"#464646FF","buttonPressedBackgroundColor":"#303030FF","buttonPressedBorderColor":"#646464FF","buttonPressedTextColor":"#969696FF","highlightColor":"#0078FFFF"},"devicePixelRatio":1,"devices":[{"id":"C439D7D7F7E1B97F76AA15D8EC41FCD4","name":"Stream Deck XL","size":{"columns":8,"rows":4},"type":2},{"id":"278C67E7A64D222ABCF3CA4837D0E2A3","name":"Google Pixel 6 Pro","size":{"columns":5,"rows":3},"type":3}],"plugin":{"uuid":"tech.aerove.streamdeck.service","version":"1.0"}}


        public StreamDeckInfo(ILogger<StreamDeckInfo> logger, List<string> startupArgs)
        {
        
            logger?.LogTrace("Startup Args Received....");
            foreach (var arg in startupArgs)
            {
                logger?.LogTrace("{arg}", arg);
            }
            try
            {
                var portIndex = startupArgs.IndexOf("-port") + 1;
                Port = int.Parse(startupArgs[portIndex]);

                var pluginUUIDIndex = startupArgs.IndexOf("-pluginUUID") + 1;
                PluginUUID = startupArgs[pluginUUIDIndex];

                var registerEventIndex = startupArgs.IndexOf("-registerEvent") + 1;
                RegisterEvent = startupArgs[registerEventIndex];

                var infoIndex = startupArgs.IndexOf("-info") + 1;
                JObject obj = JObject.Parse(startupArgs[infoIndex]);
                Info = obj.ToObject<DeviceInfo>() ?? Info;


            }
            catch (Exception e)
            {
                logger?.LogCritical(e, "Could not load config.");
            }
        }
    }



    public class DeviceInfo
    {
        public Application Application { get; set; } = new Application();
        public Colors Colors { get; set; } = new Colors();
        public int DevicePixelRatio { get; set; }
        public List<Device> Devices { get; set; } = new List<Device>();
        public Plugin Plugin { get; set; } = new Plugin();
    }

    public class Application
    {
        public string Font { get; set; } = "";
        public string Language { get; set; } = "";
        public string Platform { get; set; } = "";
        public string PlatformVersion { get; set; } = "";
        public string Version { get; set; } = "";
    }

    public class Colors
    {
        public string ButtonMouseOverBackgroundColor { get; set; } = "";
        public string ButtonPressedBackgroundColor { get; set; } = "";
        public string ButtonPressedBorderColor { get; set; } = "";
        public string ButtonPressedTextColor { get; set; } = "";
        public string HighlightColor { get; set; } = "";
    }

    public class Plugin
    {
        public string Uuid { get; set; } = "";
        public string Version { get; set; } = "";
    }

    public class Device
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public Size Size { get; set; } = new Size();
        public int Type { get; set; }
    }

    public class Size
    {
        public int Columns { get; set; }
        public int Rows { get; set; }
    }

}

