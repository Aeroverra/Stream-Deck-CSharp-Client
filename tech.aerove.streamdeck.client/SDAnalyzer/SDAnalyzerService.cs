using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.SDAnalyzer.ManifestModels;
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
        private DirectoryInfo PluginDir;
        private List<ManifestInfo> PluginManifests = new List<ManifestInfo>();
        public SDAnalyzerService(ILogger<SDAnalyzerService> logger, StreamDeckInfo streamDeckInfo)
        {
            _streamDeckInfo = streamDeckInfo;
            _logger = logger;
            try
            {
                SetPath();
                DevScanData(ProfileDir);
                ReadPluginManifests();
                var startingProfiles = ReadProfiles(ProfileDir, null);
                SetStateImagesFromPluginManifests(startingProfiles);
                MapNavigationProperties(startingProfiles);
                var multiactions = startingProfiles
                    .Where(x => x.IsFolder)
                    .Where(x => x.Actions.Count < 3)
                    .ToList();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Could not start SDAnalyzer");
                IsDisabled = true;
            }

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
                    PluginDir = directory.GetDirectories().FirstOrDefault(x => x.Name == "Plugins");
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

        /// <summary>
        /// Reads all the plugin manifests
        /// </summary>
        private void ReadPluginManifests()
        {
            foreach (var dir in PluginDir.GetDirectories())
            {
                var manifestFile = dir.GetFiles().SingleOrDefault(x => x.Name == "manifest.json");
                if (manifestFile == null || !manifestFile.Exists)
                {
                    continue;
                }
                var manifest = new ManifestInfo(manifestFile);
                PluginManifests.Add(manifest);
            }
        }

        /// <summary>
        /// Reads Elgato data from directory
        /// </summary>
        /// <param name="profilesDirectory"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private List<MProfile> ReadProfiles(DirectoryInfo profilesDirectory, MProfile? parent = null)
        {
            var profiles = new List<MProfile>();
            var profileDirectories = profilesDirectory.GetDirectories();
            foreach (var profileDirectory in profileDirectories)
            {
                var file = profileDirectory
                    .GetFiles()
                    .FirstOrDefault(x => x.Name == "manifest.json");
                if (file == null) { continue; }

                var text = File.ReadAllText(file.FullName);
                var profile = new MProfile(text, profileDirectory, parent);
                profiles.Add(profile);
                ReadActionFolders(profile);

                var path = Path.Combine(profileDirectory.FullName, $"Profiles");
                var innerProfilesDir = new DirectoryInfo(path);
                if (!innerProfilesDir.Exists) { continue; }
                var children = ReadProfiles(innerProfilesDir, profile);
                profiles.AddRange(children);
            }


            return profiles;
        }

        /// <summary>
        /// Reads the action folders of a profile for images
        /// ie: 0,1 1,0
        /// </summary>
        /// <param name="profile"></param>
        private void ReadActionFolders(MProfile profile)
        {

            var pluginActions = PluginManifests.SelectMany(x => x.Actions).ToList();
            foreach (var action in profile.Actions)
            {
                var path = Path.Combine(profile.Directory.FullName, $"{action.Col},{action.Row}/CustomImages");
                var actionDirectory = new DirectoryInfo(path);
                if (!actionDirectory.Exists) { continue; }
                foreach (var file in actionDirectory.GetFiles())
                {
                    var state = action.States.FirstOrDefault(x => x.Image == file.Name);
                    if (state == null) { continue; }
                    var base64Image = Convert.ToBase64String(File.ReadAllBytes(file.FullName));
                    var imgExtension = file.Extension.Replace(".", "");
                    state.ImageData = $"data:image/{imgExtension};base64,{base64Image}";
                    state.ImageSource = ImageSource.User;
                }

            }

        }

        /// <summary>
        /// Sets image data from plugin manifests on any state without pre-existing data
        /// </summary>
        /// <param name="profiles"></param>
        private void SetStateImagesFromPluginManifests(List<MProfile> profiles)
        {
            var actions = profiles
                .SelectMany(x => x.Actions)
                .ToList();
            var innerMultiActions = actions
                .Where(x => x.IsMultiAction)
                .SelectMany(x => x.Actions)
                .ToList();
            actions.AddRange(innerMultiActions);
            foreach (var action in actions)
            {
                var pluginManifest = PluginManifests
                    .Where(x => x.Actions.Any(y => y.Uuid == action.Uuid))
                    .SingleOrDefault();

                if (pluginManifest == null)
                {
                    _logger.LogWarning("Could not find manifest with action UUID '{uuid}'", action.Uuid);
                    continue;
                }

                var pluginManifestAction = pluginManifest.Actions.SingleOrDefault(x => x.Uuid == action.Uuid);
                if (pluginManifestAction == null)
                {
                    _logger.LogWarning("Could not find action with UUID '{uuid}' in plugin manifest", action.Uuid);
                    continue;
                }
                foreach (var state in action.States)
                {
                    if (!String.IsNullOrWhiteSpace(state.ImageData)) { continue; }
                    var stateIndex = action.States.IndexOf(state);
                    if (pluginManifestAction.States.Count < stateIndex + 1)
                    {
                        _logger.LogWarning("State defined is higher than plugin manifest. state:'{state}' action:'{action}' ", stateIndex, action.Uuid);
                        continue;
                    }
                    var pluginManifestState = pluginManifestAction.States[stateIndex];
                    if (String.IsNullOrWhiteSpace(pluginManifestState.Image))
                    {
                        _logger.LogWarning("Image not set for state '{state}' in action '{action}'", stateIndex, action.Uuid);
                        continue;
                    }
                    var imagesPath = Path.Combine(pluginManifest.DirectoryInfo.FullName, pluginManifestState.Image);
                    var imagesDir = new DirectoryInfo(imagesPath).Parent;
                    if (imagesDir == null || !imagesDir.Exists)
                    {
                        _logger.LogWarning("Image dir does not exist. state:'{state}' action:'{action}' ", stateIndex, action.Uuid);
                        continue;
                    }
                    //get image name without any paths
                    var imageName = new FileInfo(pluginManifestState.Image).Name;
                    var file = imagesDir.GetFiles().FirstOrDefault(x => x.Name.StartsWith(imageName));
                    if (file == null || !file.Exists)
                    {
                        _logger.LogWarning("Image does not exist. state:'{state}' action:'{action}' ", stateIndex, action.Uuid);
                        continue;
                    }
                    var base64Image = Convert.ToBase64String(File.ReadAllBytes(file.FullName));
                    var imgExtension = file.Extension.Replace(".", "").Replace("svg", "svg+xml");
                    state.ImageData = $"data:image/{imgExtension};base64,{base64Image}";
                    state.ImageSource = ImageSource.PluginManifest;

                }

            }


        }

        /// <summary>
        /// Maps the navigation properties like pages and folders together
        /// </summary>
        /// <param name="profiles"></param>
        private void MapNavigationProperties(List<MProfile> profiles)
        {
            //map navigation properties
            foreach (var profile in profiles)
            {
                //map pages
                if (profile.IsTopLevel && profile.IsPage)
                {
                    var nextUUID = "com.elgato.streamdeck.page.next";
                    var pages = profile.Children
                        .Where(x => x.IsPage)
                        .ToList();

                    var currentPage = profile;
                    while (currentPage != null)
                    {
                        var nextPageUUID = currentPage.Actions
                            .Where(x => x.Uuid == nextUUID)
                            .Select(x => x.Settings["ProfileUUID"].ToString())
                            .SingleOrDefault();

                        currentPage.NextPage = pages.FirstOrDefault(x => x.UUID == nextPageUUID);
                        currentPage.NextPage.PreviousPage = currentPage;
                        if (currentPage.NextPage.Actions.Any(x => x.Uuid == nextUUID))
                        {
                            currentPage = currentPage.NextPage;
                            continue;
                        }
                        currentPage = null;
                    }
                }

                //map folder parents
                var openFolderUUID = "com.elgato.streamdeck.profile.openchild";
                var openFolderButtons = profile.Actions.Where(x => x.Uuid == openFolderUUID).ToList();
                if (openFolderButtons.Count == 0) { continue; }
                foreach (var openFolderButton in openFolderButtons)
                {
                    var profileUUID = openFolderButton.Settings["ProfileUUID"]?.ToString();
                    if (profileUUID == null)
                    {
                        _logger.LogTrace("ProfileUUID null on open folder action. {profile} {button}",profile.UUID,openFolderButton.Uuid);
                        continue;
                    }
                    var buttonProfile = profiles.SingleOrDefault(x => x.UUID == profileUUID);
                    if (buttonProfile == null)
                    {
                        _logger.LogTrace("Could not find folders profile. {button} {profile}", openFolderButton.Uuid, profileUUID);
                        continue;
                    }
                    buttonProfile.Parent = profile;
                    if(!profile.Children.Any(x=>x == buttonProfile))
                    {
                        profile.Children.Add(buttonProfile);
                    }
                }

            }
        }



        /// <summary>
        /// Scans for unknown file types and folders so we can find out if something new
        /// was added.
        /// </summary>
        /// <param name="profilesDirectory"></param>
        private void DevScanData(DirectoryInfo profilesDirectory)
        {
            profilesDirectory.GetFiles()
                .ToList()
                .ForEach(x => _logger.LogWarning("Unknown file '{file}' in profiles dir", x.FullName));

            profilesDirectory.GetDirectories()
               .Where(x => !x.Name.EndsWith(".sdProfile"))
               .ToList()
               .ForEach(x => _logger.LogWarning("Unknown dir '{dir}' when reading profiles dir", x.FullName));


            foreach (var profileDirectory in profilesDirectory.GetDirectories().Where(x => x.Name.EndsWith(".sdProfile")))
            {
                profileDirectory.GetFiles()
                    .Where(x => x.Name != "manifest.json")
                    .ToList()
                    .ForEach(x => _logger.LogWarning("Unknown file '{file}' in profile dir", x.FullName));

                var hasManifest = profileDirectory.GetFiles().Any(x => x.Name == "manifest.json");
                if (!hasManifest)
                {
                    _logger.LogWarning("No manifest found in profile dir '{dir}'", profileDirectory.FullName);
                }

                profileDirectory.GetDirectories()
                    .Where(x => x.Name.Length > 3 || !x.Name.Contains(",") || !int.TryParse($"{x.Name[0]}", out int i) || !int.TryParse($"{x.Name[2]}", out int l))
                    .Where(x => x.Name != "Profiles")
                    .ToList()
                    .ForEach(x => _logger.LogWarning("Unknown dir '{dir}' in profile dir", x.FullName));

                var innerProfilesDir = profileDirectory.GetDirectories().FirstOrDefault(x => x.Name == "Profiles");
                if (innerProfilesDir != null)
                {
                    DevScanData(innerProfilesDir);
                }
                foreach (var actionDirectory in profileDirectory.GetDirectories().Where(x => x.Name != "Profiles" && x.Name.Length == 3))
                {
                    actionDirectory
                        .GetFiles()
                        .ToList()
                        .ForEach(x => _logger.LogWarning("Unknown file '{file}' in action dir", x.FullName));

                    actionDirectory
                         .GetDirectories()
                         .Where(x => x.Name != "CustomImages")
                         .ToList()
                         .ForEach(x => _logger.LogWarning("Unknown Inner dir '{Directory}' in action dir", x.FullName));


                    var imagesDirectory = actionDirectory.GetDirectories().SingleOrDefault(x => x.Name == "CustomImages");
                    if (imagesDirectory != null)
                    {
                        imagesDirectory
                          .GetFiles()
                          .Where(x => !x.Name.Contains("state"))
                          .ToList()
                          .ForEach(x => _logger.LogWarning("Unknown file '{file}' in image dir", x.FullName));

                        imagesDirectory
                             .GetDirectories()
                             .ToList()
                             .ForEach(x => _logger.LogWarning("Unknown Inner image dir '{Directory}' in image dir", x.FullName));

                    }
                }


            }
        }

    }
}
