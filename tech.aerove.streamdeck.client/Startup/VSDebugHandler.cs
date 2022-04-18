using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Startup
{
    internal static class VSDebugHandler
    {
        public static void OutputArgs(string[] args)
        {
            File.WriteAllText("args.txt", string.Join(Environment.NewLine, args));
        }
        public static string[]? DevDebug(IConfiguration config)
        {

            var elgatoPluginFolder = config.GetValue<string>("ElgatoPluginPath");
            var currentExecutablePath = Environment.ProcessPath;
            var currentExecutableFolder = Path.GetDirectoryName(currentExecutablePath);
            var executableName = Path.GetFileName(currentExecutablePath);
            if (elgatoPluginFolder == currentExecutableFolder)
            {
                Console.WriteLine("DevDebug is on within the plugins folder. This should not happen.");
                return null;
            }
            var newArgs = MoveFiles(elgatoPluginFolder, currentExecutableFolder, executableName);
            return newArgs;

        }
        private static string[]? MoveFiles(string elgatoPath, string currentPath, string executableName)
        {
            File.Delete($"{elgatoPath}\\args.txt");
            File.Delete($"{elgatoPath}\\appsettings.json");
            File.Delete($"{elgatoPath}\\appsettings.Development.json");
            File.Delete($"{elgatoPath}\\manifest.json");
            File.WriteAllText($"{elgatoPath}\\appsettings.json", "{\"DevLogParametersOnly\":true}");
            File.Copy($"{currentPath}\\manifest.json", $"{elgatoPath}\\manifest.json");

            var process = Process.GetProcesses()
                .Where(x => x.MatchesPath(elgatoPath))
                .Where(x => x.HasFileName(executableName))
                .SingleOrDefault();

            if (process != null)
            {
                process.Kill();
            }

            Console.WriteLine("Reading new args");
            for (int x = 0; x < 10; x++)
            {
                if (!File.Exists($"{elgatoPath}\\args.txt"))
                {
                    Thread.Sleep(x*500);
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
