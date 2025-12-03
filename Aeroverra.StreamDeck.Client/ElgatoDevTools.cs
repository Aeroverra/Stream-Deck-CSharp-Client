using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Aeroverra.StreamDeck.Client
{
    public static class ElgatoDevTools
    {
        private const string RegistryKeyPath = @"Software\Elgato Systems GmbH\StreamDeck";
        private const string FlagName = "developer_mode";

        public static void SetDeveloperMode(bool enable)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath);
                key.SetValue(FlagName, enable ? 1 : 0, RegistryValueKind.DWord);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "defaults",
                    ArgumentList = { "write", "com.elgato.StreamDeck", FlagName, "-bool", enable ? "YES" : "NO" },
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var proc = Process.Start(psi);
                proc!.WaitForExit();
                if (proc.ExitCode != 0)
                {
                    throw new InvalidOperationException($"defaults exited with {proc.ExitCode}");
                }
            }
            else
            {
                throw new PlatformNotSupportedException("Stream Deck developer mode toggle is only defined for Windows and macOS.");
            }
        }

        private const string PluginSuffix = ".sdPlugin";

        public static void RestartPlugin(string uuid, bool startIfNotRunning = true)
        {
            if (!IsPluginInstalled(uuid))
                throw new InvalidOperationException($"Plugin '{uuid}' is not installed.");

            if (!IsStreamDeckRunning())
            {
                if (!startIfNotRunning)
                    return;

                RunStreamDeckUrl($"streamdeck://plugins/restart/{uuid}");
                return;
            }

            RunStreamDeckUrl($"streamdeck://plugins/restart/{uuid}");
        }

        public static void StopPlugin(string uuid)
        {
            if (!IsPluginInstalled(uuid))
                throw new InvalidOperationException($"Plugin '{uuid}' is not installed.");

            if (!IsStreamDeckRunning())
            {
                Console.WriteLine("Stream Deck is not running.");
                return;
            }

            RunStreamDeckUrl($"streamdeck://plugins/stop/{uuid}");
        }

        public static bool IsPluginInstalled(string uuid)
        {
            var pluginsPath = GetPluginsPath();
            if (!Directory.Exists(pluginsPath))
                return false;

            var expectedName = uuid + PluginSuffix;
            return Directory.EnumerateFileSystemEntries(pluginsPath)
                .Select(Path.GetFileName)
                .Any(name => name != null && name.Equals(expectedName, StringComparison.OrdinalIgnoreCase));
        }

        public static string GetPluginsPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(home, "Library", "Application Support", "com.elgato.StreamDeck", "Plugins");
            }

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "Elgato", "StreamDeck", "Plugins");
        }

        private static bool IsStreamDeckRunning()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return Process.GetProcessesByName("Elgato Stream Deck").Any();

            return Process.GetProcessesByName("StreamDeck").Any();
        }

        private static void RunStreamDeckUrl(string url)
        {
            ProcessStartInfo psi;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                psi = new ProcessStartInfo("open")
                {
                    ArgumentList = { "-g", url },
                    UseShellExecute = false
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
            }
            else
            {
                throw new PlatformNotSupportedException("Only Windows and macOS are supported.");
            }

            using var process = Process.Start(psi);
            process?.Dispose();
        }
    }
}
