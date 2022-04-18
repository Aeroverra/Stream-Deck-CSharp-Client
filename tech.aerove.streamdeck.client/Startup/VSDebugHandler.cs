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
        public static void DevDebug(IConfiguration config)
        {

            var elgatoPluginFolder = config.GetValue<string>("ElgatoPluginPath");
            var currentExecutablePath = Environment.ProcessPath;
            var currentExecutableFolder = Path.GetDirectoryName(currentExecutablePath);
            var executableName = Path.GetFileName(currentExecutablePath);
            if (elgatoPluginFolder == currentExecutableFolder)
            {
                return;
            }
            MoveFiles(elgatoPluginFolder, currentExecutableFolder, executableName);


        }
        private static void MoveFiles(string elgatoPath, string currentPath, string executableName)
        {
            var allElgatoFiles = Directory.GetFiles(elgatoPath, "*", SearchOption.AllDirectories);
            List<string> FilesNotDeleted = new List<string>();
            foreach (var file in allElgatoFiles)
            {
                Console.WriteLine($"[DELERR] {file}");
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    FilesNotDeleted.Add(file);
                }
            }

            var currentFiles = Directory.GetFiles(currentPath, "*", SearchOption.AllDirectories);
            var filesPendingCopy = new List<string>();
            foreach (var file in currentFiles)
            {
                var fileName = Path.GetFileName(file);
                if (FilesNotDeleted.Where(x => x.EndsWith(fileName)).Any())
                {
                    filesPendingCopy.Add(file);
                    continue;
                }
                var extraPath = Path.GetDirectoryName(file).Replace(currentPath, "");
                var copyPath = $"{elgatoPath}{extraPath}\\{fileName}";
                Directory.CreateDirectory(Path.GetDirectoryName(copyPath));
                File.Copy(file, copyPath);
            }

            File.Delete($"{elgatoPath}\\appsettings.Development.json");
            File.Delete($"{elgatoPath}\\appsettings.json");
            File.WriteAllText($"{elgatoPath}\\appsettings.json", "{\"DevLogParametersOnly\":true}");

            var process = Process.GetProcesses()
                .Where(x => x.MatchesPath(elgatoPath))
                .Where(x => x.HasFileName(executableName))
                .SingleOrDefault();

            if (process != null)
            {
                process.Kill();
            }

            foreach (var file in FilesNotDeleted)
            {
                try
                {
                    File.Delete(file);
                    Console.WriteLine("success");
                }
                catch (Exception e)
                {
                    FilesNotDeleted.Add(file);
                }
            }
            foreach (var file in filesPendingCopy)
            {
                var fileName = Path.GetFileName(file);
                var extraPath = Path.GetDirectoryName(file).Replace(currentPath, "");
                var copyPath = $"{elgatoPath}{extraPath}\\{fileName}";
                Directory.CreateDirectory(Path.GetDirectoryName(copyPath));
                File.Copy(file, copyPath);
            }

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
