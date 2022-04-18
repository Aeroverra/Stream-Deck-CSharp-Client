using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using tech.aerove.streamdeck.client.Actions;
using tech.aerove.streamdeck.client.Cache;
using tech.aerove.streamdeck.client.Events;
using tech.aerove.streamdeck.client.Startup;

namespace tech.aerove.streamdeck.client
{
    public static class IServiceCollectionExtensions
    {
        public static void AddAeroveStreamDeckClient(this IServiceCollection services, HostBuilderContext context, string[] args)
        {
            var config = context.Configuration;
            var logParametersOnly = config.GetValue<bool>("DevLogParametersOnly");
            if (logParametersOnly)
            {
                VSDebugHandler.OutputArgs(args);
            }
            var devDebug = config.GetValue<bool>("DevDebug");
            if (devDebug)
            {
                var newArgs = VSDebugHandler.DevDebug(config);
                args = newArgs == null?args:newArgs;
            }


            services.AddSingleton<WebSocketService>();
            services.AddHostedService<WebSocketService>(provider => provider.GetService<WebSocketService>());


            services.AddSingleton<IElgatoEventHandler, DefaultElgatoEventHandler>();
            services.AddSingleton<IElgatoDispatcher, DefaultElgatoDispatcher>();
            services.AddTransient<ManifestInfo>();


            services.AddSingleton<StreamDeckInfo>(x => new StreamDeckInfo(x.GetRequiredService<ILogger<StreamDeckInfo>>(), args.ToList()));

            services.AddSingleton<IActionFactory, DefaultActionFactory>();
            services.AddSingleton<IActionExecuter, DefaultActionExecuter>();
            services.AddSingleton<ICache, DefaultCache>();

        }


    }
}