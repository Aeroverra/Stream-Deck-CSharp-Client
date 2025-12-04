using Aeroverra.StreamDeck.Client.Events;
using Aeroverra.StreamDeck.Client.SDAnalyzer.ManifestModels;
using Aeroverra.StreamDeck.Client.Startup;
using Microsoft.Extensions.Logging;

namespace Aeroverra.StreamDeck.Client.SDAnalyzer
{
    internal class SDAnalyzerService
    {
        private readonly StreamDeckInfo _streamDeckInfo;
        private readonly ILogger<SDAnalyzerService> _logger;
        private bool IsDisabled = false;
        private DirectoryInfo ProfileDir;
        private DirectoryInfo PluginDir;
        private List<ManifestInfo> PluginManifests = new List<ManifestInfo>();
        private List<MProfile> LoadedProfiles = new List<MProfile>();
        private List<String> PluginActionUUIDs = new List<String>();
        public SDAnalyzerService(ILogger<SDAnalyzerService> logger, StreamDeckInfo streamDeckInfo, ManifestInfo manifestInfo)
        {
            PluginActionUUIDs = manifestInfo.Actions.Select(x => x.Uuid).ToList();
            _streamDeckInfo = streamDeckInfo;
            _logger = logger;
            try
            {
                SetPath();
                DevScanData(ProfileDir);
                ReadPluginManifests();
                LoadedProfiles = ReadProfiles(ProfileDir, null);
                MapNavigationProperties(LoadedProfiles);

                var actions = LoadedProfiles.SelectMany(x => x.Actions).ToList();
                var multiActions = actions.Where(x => x.IsMultiAction).SelectMany(x => x.Actions).ToList();
                actions.AddRange(multiActions);
                var states = actions.SelectMany(x => x.States).ToList();
                var withoutImg = states.Where(x => x.ImageSource == ImageSource.Unknown).ToList();

            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Could not start SDAnalyzer");
                IsDisabled = true;
            }

        }

        private void SetPath()
        {
            var DataPath = "";
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
                manifest.Setup();
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
                var profile = new MProfile(text, profileDirectory, PluginManifests, parent);
                profiles.Add(profile);

                var path = Path.Combine(profileDirectory.FullName, $"Profiles");
                var innerProfilesDir = new DirectoryInfo(path);
                if (!innerProfilesDir.Exists) { continue; }
                var children = ReadProfiles(innerProfilesDir, profile);
                profiles.AddRange(children);
            }


            return profiles;
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
                        _logger.LogTrace("ProfileUUID null on open folder action. {profile} {button}", profile.UUID, openFolderButton.Uuid);
                        continue;
                    }
                    var buttonProfile = profiles.SingleOrDefault(x => x.UUID == profileUUID);
                    if (buttonProfile == null)
                    {
                        _logger.LogTrace("Could not find folders profile. {button} {profile}", openFolderButton.Uuid, profileUUID);
                        continue;
                    }
                    buttonProfile.Parent = profile;
                    if (!profile.Children.Any(x => x == buttonProfile))
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


        public List<MProfile> HandleFirstLoad(DidReceiveGlobalSettingsEvent e)
        {
            var settings = e.Payload.Settings["AeroveSDAnalyzer"]?.ToObject<List<MProfile>>();
            if (settings == null || settings.Count == 0)
            {
                return LoadedProfiles;
            }
            return LoadedProfiles;
        }

    }
}
