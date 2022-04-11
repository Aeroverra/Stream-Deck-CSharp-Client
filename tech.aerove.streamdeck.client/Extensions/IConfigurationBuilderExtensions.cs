using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client
{
    public static class IConfigurationBuilderExtensions
    {
        //Parses the args and adds them to our configuration
        public static IConfigurationBuilder AddStreamDeckArgs(this IConfigurationBuilder configurationBuilder, string[] args)
        {
            var newArgs = new List<string>();
            var allArgs = string.Join(",,,", args);
            allArgs = "args=" + allArgs;
            newArgs.Add(allArgs);
            for (int x = 0; x < args.Length; x++)
            {
                try
                {
                    //handled in a way that allows for other args to be passed just in case
                    var key = args[x];
                    //check to make sure this is a stream deck arg (starts with a single -)
                    if (key.StartsWith("-") && !key.StartsWith("--"))
                    {
                        key = key.Replace("-", "");
                    }
                    //check to make sure we don't have another key in the event a value was missing
                    if (x + 1 < args.Length && !args[x + 1].StartsWith("-"))
                    {
                        var value = args[x + 1];
                        newArgs.Add($"{key}={value}");
                        x++; //skip next one because we used it as the value
                    }

                }
                catch (Exception e)
                {
                    //just in case
                    Console.WriteLine("Could not parse args!");
                    Console.WriteLine(e);
                }
            }
            return configurationBuilder.AddCommandLine(newArgs.ToArray());

        }
    }
}
