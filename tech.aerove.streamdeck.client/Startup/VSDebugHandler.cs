using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Startup
{
    /// <summary>
    /// Magic behind the no restart easy debug by Aeroverra
    /// </summary>
    internal static class VSDebugHandler
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
        /// Checks if DevDebug is true and if so updates the necessary files in the
        /// elgato plugin folder including a new appsettings setting 
        /// DevLogParametersOnly to true. The plugin process is then killed causing 
        /// the stream deck to restart the process. The DevLogParametersOnly outputs
        /// the newly passed args to a txt file and prevents the socket from connecting.
        /// The actual child process remains running but not functioning allowing
        /// us to debug and connect with those args.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string[]? DevDebug(IConfiguration config)
        {
            var devDebug = config.GetValue<bool>("DevDebug");
            if (!devDebug)
            {
                return null;
            }
            var elgatoPluginFolder = config.GetValue<string>("ElgatoPluginPath");
            var currentExecutablePath = Environment.ProcessPath;
            var currentExecutableFolder = Path.GetDirectoryName(currentExecutablePath);
            var executableName = Path.GetFileName(currentExecutablePath);
            if (elgatoPluginFolder == currentExecutableFolder)
            {
                Console.WriteLine("DevDebug is on within the plugins folder. This should not happen.");
                return null;
            }
            var newArgs = UpdateFiles(elgatoPluginFolder, currentExecutableFolder, executableName);
            if (newArgs == null)
            {
                var success = RestartStreamDeckAndMoveNewFiles(elgatoPluginFolder, currentExecutableFolder);
                if (success)
                {
                    newArgs = UpdateFiles(elgatoPluginFolder, currentExecutableFolder, executableName);
                    if (newArgs != null)
                    {
                        Console.WriteLine("Successfully recovered from arg read error.");
                    }
                }
            }
            return newArgs;

        }
       
        private static bool RestartStreamDeckAndMoveNewFiles(string elgatoPath, string currentPath)
        {
            Console.WriteLine("Attempting to fix devdebug by restarting stream deck and copying new files.");

            var streamdeckProcess = Process.GetProcesses()
               .Where(x => x.HasFileName("StreamDeck.exe"))
               .FirstOrDefault();


            if (streamdeckProcess == null)
            {
                Console.WriteLine("Stream Deck is not running. Stream Deck must be running to use devdebug.");
                return false;
            }
            var streamdeckExecutable = streamdeckProcess.MainModule.FileName;
            streamdeckProcess.Kill();

            var allElgatoFiles = Directory.GetFiles(elgatoPath, "*", SearchOption.AllDirectories);
            foreach (var file in allElgatoFiles)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[DELETE ERROR] {file}");
                    return false;
                }
            }

            var currentFiles = Directory.GetFiles(currentPath, "*", SearchOption.AllDirectories);
            foreach (var file in currentFiles)
            {
                var fileName = Path.GetFileName(file);
                var extraPath = Path.GetDirectoryName(file).Replace(currentPath, "");
                var copyPath = $"{elgatoPath}{extraPath}\\{fileName}";
                Directory.CreateDirectory(Path.GetDirectoryName(copyPath));
                File.Copy(file, copyPath);
            }


            streamdeckProcess = Process.Start(streamdeckExecutable);

            return true;
        }
        
        private static string[]? UpdateFiles(string elgatoPath, string currentPath, string executableName, bool secondAttempt = false)
        {
            Directory.CreateDirectory(elgatoPath);
            File.Delete($"{elgatoPath}\\args.txt");
            File.Delete($"{elgatoPath}\\appsettings.json");
            File.Delete($"{elgatoPath}\\appsettings.Development.json");
            File.Delete($"{elgatoPath}\\manifest.json");
            File.WriteAllText($"{elgatoPath}\\appsettings.json", "{\"DevLogParametersOnly\":true}");
            File.Copy($"{currentPath}\\manifest.json", $"{elgatoPath}\\manifest.json");

            var piDirectory = new DirectoryInfo($"{currentPath}\\PropertyInspector");
            if (piDirectory.Exists)
            {
                var propertyInspectorFiles = Directory.GetFiles(piDirectory.FullName, "*", SearchOption.AllDirectories);
                foreach (var file in propertyInspectorFiles)
                {
                    var fileName = Path.GetFileName(file);
                    var extraPath = Path.GetDirectoryName(file).Replace($"{currentPath}\\PropertyInspector", "");
                    var copyPath = $"{elgatoPath}\\PropertyInspector{extraPath}\\{fileName}";
                    Directory.CreateDirectory(Path.GetDirectoryName(copyPath));
                    File.Copy(file, copyPath, true);
                }
            }
          

            var process = Process.GetProcesses()
                .Where(x => x.MatchesPath(elgatoPath))
                .Where(x => x.HasFileName(executableName))
                .SingleOrDefault();

            if (process != null)
            {
                process.Kill();
            }

            Console.WriteLine("Reading new args");
            //waits for a max of 11250 ms first attempt
            var multiplier = 250;
            //takes longer to restart stream deck
            if (secondAttempt) { multiplier = 1000; }
            for (int x = 0; x < 10; x++)
            {
                if (!File.Exists($"{elgatoPath}\\args.txt"))
                {
                    Thread.Sleep(x * multiplier);
                    continue;
                }
                Console.WriteLine("Args read successfully!");
                return File.ReadAllLines($"{elgatoPath}\\args.txt");
            }
            Console.WriteLine("Could not read new args!");
            return null;
        }

        private static bool MatchesPath(this Process process, string path)
        {
            try
            {
                var fileName = process.MainModule?.FileName;
                if (fileName != null && fileName.StartsWith(path.Replace("/", "\\")))
                {
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        private static bool HasFileName(this Process process, string executableName)
        {
            try
            {
                var fileName = process.MainModule?.FileName;
                if (fileName != null && fileName.EndsWith(executableName))
                {
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }
    }
}
