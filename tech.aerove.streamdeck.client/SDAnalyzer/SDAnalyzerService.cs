using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.SDAnalyzer.Models;
using tech.aerove.streamdeck.client.Startup;

namespace tech.aerove.streamdeck.client.SDAnalyzer
{
    internal class SDAnalyzerService
    {
        private readonly StreamDeckInfo _streamDeckInfo;
        private readonly ILogger<SDAnalyzerService> _logger;
        private string DataPath = "";
        private bool IsDisabled = false;
        private DirectoryInfo ProfileDir;

        public SDAnalyzerService(ILogger<SDAnalyzerService> logger, StreamDeckInfo streamDeckInfo)
        {
            _streamDeckInfo = streamDeckInfo;
            _logger = logger;
            SetPath();
            ReadData();
        }

        private void SetPath()
        {
            try
            {
                var currentExecutablePath = Environment.ProcessPath;
                var currentExecutableFolder = Path.GetDirectoryName(currentExecutablePath);
                var executableName = Path.GetFileName(currentExecutablePath);
                var parent = Directory.GetParent(currentExecutableFolder).Parent;
                var directories = parent.GetDirectories().ToList();
                if (directories.Any(x => x.Name == "ProfilesV2") && directories.Any(x => x.Name == "Plugins"))
                {
                    DataPath = parent.FullName;
                    var directory = new DirectoryInfo(DataPath);
                    ProfileDir = directory.GetDirectories().FirstOrDefault(x => x.Name == "ProfilesV2");

                    return;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Error when trying to find data path by current executable");
            }
            try
            {
                if (_streamDeckInfo.Info.Application.Platform.ToLower().Contains("mac"))
                {
                    var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    path = Path.Combine(path, "Library/Application Support/com.elgato.StreamDeck/");
                    DataPath = path;
                }
                else
                {
                    DataPath = Environment.ExpandEnvironmentVariables("%appdata%\\Elgato\\StreamDeck\\");

                }
                var dirs = new DirectoryInfo(DataPath).GetDirectories();
                if (dirs.Any(x => x.Name == "ProfilesV2") && dirs.Any(x => x.Name == "Plugins"))
                {
                    var directory = new DirectoryInfo(DataPath);
                    ProfileDir = directory.GetDirectories().FirstOrDefault(x => x.Name == "ProfilesV2");

                    return;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Error when trying to find data path with defeault value.");
            }
            _logger.LogCritical("SDAnalyzer Failed To Start.");
            IsDisabled = true;
        }

        private void ReadData()
        {
            var profileFolders = ProfileDir.GetDirectories();
            foreach (var profile in profileFolders)
            {
                var file = profile.GetFiles().FirstOrDefault(x => x.Name == "manifest.json");
                var text = File.ReadAllText(file.FullName);
                var profileObj = JsonConvert.DeserializeObject<ProfileManifest>(text);
            }
        }
    }
}
