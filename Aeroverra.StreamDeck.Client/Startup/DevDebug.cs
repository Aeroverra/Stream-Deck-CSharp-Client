using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;

namespace Aeroverra.StreamDeck.Client.Startup
{
    /// <summary>
    /// Magic behind the no restart easy debug by Aeroverra
    /// </summary>
    internal static class DevDebug
    {
        /// <summary>
        /// Checks if DevLogParametersOnly is set to true and if so
        /// outputs a file with the starting args to be used by the debugging app.
        /// The websocket will also not be opened.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="args">Startup args</param>
        public static void OutputArgs(IConfiguration config)
        {
            var logParametersOnly = config.GetValue<bool>("DevLogParametersOnly");
            if (!logParametersOnly)
            {
                return;
            }
            string[] args = Environment.GetCommandLineArgs();
            File.WriteAllText("args.txt", string.Join(Environment.NewLine, args));
        }

        /// <summary>
        /// Checks if DevDebug is true and if so attempts to takeover elgato plugin connection.
        /// </summary>
        /// <returns>New args from takeover</returns>
        public static string[]? TakeOver(IConfiguration config)
        {
            Console.WriteLine("DevDebug takeover started.");

            var devDebug = config.GetValue<bool>("DevDebug");
            var logParametersOnly = config.GetValue<bool>("DevLogParametersOnly");

            if (devDebug == false || logParametersOnly == true)
                return null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Console.WriteLine("DevDebug is untested on mac. Let me know if it works or doesn't work. https://discord.aerove.tech");
            }

            ElgatoDevTools.SetDeveloperMode(true);

            var pluginUUID = config.GetValue<string>("UUID");
            if (string.IsNullOrEmpty(pluginUUID))
            {
                Console.WriteLine("DevDebug requires a UUID to be set in appsettings.json.");
                Environment.Exit(0);
            }

            var installed = ElgatoDevTools.IsPluginInstalled(pluginUUID);
            if (installed == true)
            {
                ElgatoDevTools.StopPlugin(pluginUUID);
            }

            var elgatoPluginsPath = ElgatoDevTools.GetPluginsPath();
            var pluginPath = Path.Combine(elgatoPluginsPath, $"{pluginUUID}.sdPlugin");
            var pluginDirectory = new DirectoryInfo(pluginPath);

            UpdateFiles(pluginDirectory);

            ElgatoDevTools.RestartPlugin(pluginUUID);

            if (ReadArgs(pluginDirectory, out string[]? args, true))
            {
                Console.WriteLine("DevDebug recovery and takeover success!");
                return args;
            }

            Console.WriteLine("DevDebug failed. Follow these steps then try again." +
                "\r\n\t1.) Kill the Stream Deck application." +
                "\r\n\t2.) Delete the plugin folder for this plugin within the Elgato plugins folder." +
                "\r\n\t3.) Start the Stream Deck application." +
                "\r\n\t4.) Run your plugin in debug mode." +
                "\r\n\t5.) If its still not working please let me know. https://discord.aerove.tech");

            Environment.Exit(0);

            return null;
        }

        private static void UpdateFiles(DirectoryInfo pluginDirectory)
        {
            var processPath = Environment.ProcessPath;

            if (processPath == null)
                throw new Exception("Process path returned null. Are you running this on a toaster?");

            var currentExecutable = new FileInfo(processPath);

            var elgatoDirectoryExists = pluginDirectory.Exists;

            currentExecutable.Directory!.TryCopyContents(pluginDirectory, true);

            var appSettingsFilePath = Path.Combine(pluginDirectory.FullName, "appsettings.json");

            File.WriteAllText($"{appSettingsFilePath}", "{\"DevLogParametersOnly\":true}");

            var argsPath = Path.Combine(pluginDirectory.FullName, "args.txt");
            if (File.Exists(argsPath))
            {
                File.Delete(argsPath);
            }
        }

        private static bool ReadArgs(DirectoryInfo elgatoPluginDir, out string[]? args, bool longWait = false)
        {
            var argsPath = Path.Combine(elgatoPluginDir.FullName, "args.txt");
            //multiplier 250  = 11250 ms
            var multiplier = 250;
            //takes longer to restart stream deck
            if (longWait) { multiplier = 1000; }
            for (int x = 0; x < 10; x++)
            {
                if (!File.Exists(argsPath))
                {
                    Thread.Sleep(x * multiplier);
                    continue;
                }
                args = File.ReadAllLines(argsPath);
                File.Delete(argsPath);
                if (args.Length < 4)
                {
                    Console.WriteLine("DevDebug failed takeover.");
                    return false;
                }
                return true;
            }
            Console.WriteLine("DevDebug failed takeover.");
            args = null;
            return false;
        }
    }
}
