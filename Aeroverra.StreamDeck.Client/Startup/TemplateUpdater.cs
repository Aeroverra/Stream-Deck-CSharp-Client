using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Aeroverra.StreamDeck.Client.Startup
{
    internal static class TemplateUpdater
    {
        public static void UpdateTemplate(IConfiguration config)
        {
            var devDebug = config.GetValue<bool>("DevDebug", false);
            if (!devDebug)
            {
                return;
            }
            try
            {
                var listResponse = ExecuteDotnetCommand("dotnet new --list");
                var templateInstalled = listResponse.Where(x => x.ToLower().Contains("stream deck plugin")).Any();
                if (!templateInstalled)
                {
                    ExecuteDotnetCommand("dotnet new -i Tech.Aerove.StreamDeck.Template");
                }
                var updateCheckResponse = ExecuteDotnetCommand("dotnet new --update-check");
                var needsUpdate = updateCheckResponse.FirstOrDefault(x => x.ToLower().Contains("dotnet new --install tech.aerove.streamdeck.template"));
                if (needsUpdate != null && needsUpdate.Contains("dotnet new --install"))
                {
                    var updateResponse = ExecuteDotnetCommand(needsUpdate.Trim());
                    var updateSuccess = updateResponse.Where(x => x.ToLower().Contains("success: tech.aerove.streamdeck.template")).Any();
                    if (updateSuccess)
                    {
                        Console.WriteLine("Plugin Template Updated.");
                    }
                    else
                    {
                        Console.WriteLine("Failed To Update Plugin Template.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Template update failed: {e.Message}");
            }
        }
        private static List<string> ExecuteDotnetCommand(string command)
        {
            List<string> response = new List<string>();
            var process = new Process();
            var aeroProcessStarted = false;
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet.exe",
                    Arguments = $"{command}",
                    RedirectStandardOutput = true,
                };

                process.StartInfo = startInfo;
                aeroProcessStarted = process.Start();

                while (!process.StandardOutput.EndOfStream)
                {
                    response.Add(process.StandardOutput.ReadLine());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"dotnet command '{command}' failed: {e.Message}");
            }
            if (aeroProcessStarted && process.HasExited == false)
            {
                process.Kill();
            }
            return response;

        }
    }
}
