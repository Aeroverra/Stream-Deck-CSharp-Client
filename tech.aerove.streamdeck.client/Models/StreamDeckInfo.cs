using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Models
{
    public class StreamDeckInfo
    {

        public int Port { get; set; }
        public string PluginUUID { get; set; } = "";
        public string RegisterEvent { get; set; } = "";
        public DeviceInfo Info { get; set; } = new DeviceInfo();



        public StreamDeckInfo(ILogger<StreamDeckInfo> logger, IConfiguration config)
        {
            try
            {
                config.Bind(this);
                var info = config["info"];
                JObject obj = JObject.Parse(info);
                Info = obj.ToObject<DeviceInfo>() ?? Info;
                logger.LogInformation($"Port:{Port}");
                var args = config["args"].Split(",,,");
                foreach (var arg in args)
                {
                    logger.LogInformation("{arg}", arg);
                }
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Could not load config.");
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
