using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using tech.aerove.streamdeck.client.Actions;
using tech.aerove.streamdeck.client.Cache;
using tech.aerove.streamdeck.client.Events;
using tech.aerove.streamdeck.client.Startup;

namespace tech.aerove.streamdeck.client
{
    public static class IServiceCollectionExtensions
    {
        public static void AddAeroveStreamDeckClient(this IServiceCollection services)
        {
            services.AddSingleton<WebSocketService>();
            services.AddHostedService<WebSocketService>(provider => provider.GetService<WebSocketService>());

            services.AddSingleton<StreamDeckInfo>();
            services.AddSingleton<IElgatoEventHandler, DefaultElgatoEventHandler>();
            services.AddSingleton<IElgatoDispatcher, DefaultElgatoDispatcher>();
            services.AddTransient<ManifestInfo>();
            services.AddSingleton<IActionFactory, DefaultActionFactory>();
            services.AddSingleton<IActionExecuter,DefaultActionExecuter>();
            services.AddSingleton<ICache, DefaultCache>();
            //services.TryAddSingleton<IActionExecuter, DefaultActionExecuter>();

        }


    }
}