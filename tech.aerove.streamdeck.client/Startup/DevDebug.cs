using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tech.Aerove.StreamDeck.Client.Startup
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
            var devDebug = config.GetValue<bool>("DevDebug");
            if (!devDebug)
            {
                return null;
            }
            var elgatoPluginDir = new DirectoryInfo(Environment.ExpandEnvironmentVariables(config.GetValue<string>("ElgatoPluginPath")));
            if (config.GetValue<string>("ElgatoPluginPath").Contains("~/Library"))
            {
                //Mac
                Console.WriteLine("DevDebug is untested on mac. Let me know if it works or doesn't work. https://discord.aerove.tech");
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                path = Path.Combine(path, "Library/Application Support/com.elgato.StreamDeck/Plugins/");
                elgatoPluginDir = new DirectoryInfo(path);
            }
            var currentExecutable = new FileInfo(Environment.ProcessPath);
            if (elgatoPluginDir.FullName == currentExecutable.Directory.FullName)
            {
                Console.WriteLine("DevDebug is on within the plugins folder. This should not happen.");
                return null;
            }
            var streamDeckExecutable = GetStreamDeckExecutable();
            if (streamDeckExecutable == null)
            {
                Console.WriteLine("Your Stream Deck must be running to use DevDebug.");
                Environment.Exit(0);
                return null;

            }
            Console.WriteLine("DevDebug takeover started.");
            if (OutputArgsIsSetup(elgatoPluginDir) && OutputArgsExist(elgatoPluginDir) && ReadArgs(elgatoPluginDir, out string[] args))
            {
                UpdateFiles(elgatoPluginDir, streamDeckExecutable);
                Console.WriteLine("DevDebug takeover success!");
                return args;
            }
            var restartedSD = UpdateFiles(elgatoPluginDir, streamDeckExecutable);
            if (ReadArgs(elgatoPluginDir, out string[] args2, restartedSD))
            {
                Console.WriteLine("DevDebug takeover success!");
                return args2;
            }
            Console.WriteLine("DevDebug failed takeover... Trying to recover...");
            UpdateFiles(elgatoPluginDir, streamDeckExecutable, true);
            if (ReadArgs(elgatoPluginDir, out string[] args3, true))
            {
                Console.WriteLine("DevDebug recovery and takeover success!");
                return args3;
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

        private static bool UpdateFiles(DirectoryInfo elgatoPluginDir, FileInfo streamDeckExecutable, bool forceKill = false)
        {
            if (forceKill)
            {
                KillStreamDeck(streamDeckExecutable, false);
                elgatoPluginDir.Delete(true);
            }
            var currentExecutable = new FileInfo(Environment.ProcessPath);
            var elgatoDirectoryExists = elgatoPluginDir.Exists;
            currentExecutable.Directory.TryCopyContents(elgatoPluginDir, true);
            var appSettingsFilePath = Path.Combine(elgatoPluginDir.FullName, "appsettings.json");
            File.WriteAllText($"{appSettingsFilePath}", "{\"DevLogParametersOnly\":true}");

            if (!elgatoDirectoryExists)
            {
                Console.WriteLine("Directory did not exist. Restart required. Restarting Now...");
                KillStreamDeck(streamDeckExecutable, true);
                return true;
            }
            else
            {
                KillPluginProcess();
                return false;
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

        private static bool OutputArgsIsSetup(DirectoryInfo elgatoPluginDir)
        {
            if (!elgatoPluginDir.Exists) { return false; }

            var manifestFile = elgatoPluginDir
                .GetFiles()
                .FirstOrDefault(x => x.Name == "appsettings.json");

            if (manifestFile == null) { return false; }
            var manifest = File.ReadAllText(manifestFile.FullName);
            var manifestObj = JObject.Parse(manifest);
            var outputParam = manifestObj["DevLogParametersOnly"];
            if (outputParam == null || outputParam.ToObject<bool>() == false) { return false; }

            return true;
        }

        private static bool OutputArgsExist(DirectoryInfo elgatoPluginDir)
        {
            var argsPath = Path.Combine(elgatoPluginDir.FullName, "args.txt");
            var argsFile = new FileInfo(argsPath);
            if (argsFile.Exists)
            {
                return true;
            }
            return false;
        }

        private static FileInfo? GetStreamDeckExecutable()
        {
            var process = Process
                .GetProcesses()
                .SafeOnly()
                .Where(x => !x.HasExited)
                .Where(x => x.MainModule.FileName.EndsWith("StreamDeck.exe") || x.MainModule.FileName.EndsWith("StreamDeck"))
                .FirstOrDefault();
            if (process == null)
            {
                return null;
            }
            return new FileInfo(process.MainModule.FileName);

        }

        private static void KillStreamDeck(FileInfo streamDeckExecutable, bool restart = true)
        {
            var processes = Process.GetProcesses().SafeOnly();

            var elgatoProcesses = processes
               .Where(x => !x.HasExited)
               .Where(x => x.MainModule.FileName.EndsWith(streamDeckExecutable.Name))
               .ToList();

            elgatoProcesses.ForEach(x => x.Kill());

            var currentExecutable = new FileInfo(Environment.ProcessPath);
            var pid = Environment.ProcessId;
            var pluginProcesses = processes
                .Where(x => !x.HasExited)
                .Where(x => x.Id != pid)
                .Where(x => x.MainModule.FileName.EndsWith(currentExecutable.Name))
                .ToList();

            pluginProcesses.ForEach(x => x.Kill());

            if (restart)
            {
                Process.Start(streamDeckExecutable.FullName);
            }
        }

        private static void KillPluginProcess()
        {
            var processes = Process.GetProcesses().SafeOnly();
            var currentExecutable = new FileInfo(Environment.ProcessPath);
            var pid = Environment.ProcessId;
            var pluginProcesses = processes
              .Where(x => !x.HasExited)
              .Where(x => x.Id != pid)
              .Where(x => x.MainModule.FileName.EndsWith(currentExecutable.Name))
              .ToList();
            pluginProcesses.ForEach(x => x.Kill());
        }



    }
}
