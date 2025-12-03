using System.Diagnostics;

namespace Aeroverra.StreamDeck.Client.Extensions
{
    internal static class ProcessExtensions
    {
        public static List<Process> SafeOnly(this Process[] processes)
        {
            List<Process> processList = new List<Process>();
            foreach (var process in processes)
            {
                try
                {
                    var module = process.MainModule;
                    processList.Add(process);
                }
                catch
                {

                }
            }
            return processList;
        }
    }
}
