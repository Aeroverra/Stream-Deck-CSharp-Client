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

        public static IConfigurationBuilder AddStreamDeckArgs(this IConfigurationBuilder configurationBuilder, string[] args)
        {
            var newArgs = new List<string>();
            for (int x = 0; x < args.Length; x += 2)
            {
                var key = args[x].Replace("-", "");
                var value = args[x + 1];
                newArgs.Add($"{key}={value}");
            }
            newArgs.ForEach(x => Console.WriteLine(x));
            return configurationBuilder.AddCommandLine(newArgs.ToArray());

        }
    }
}
