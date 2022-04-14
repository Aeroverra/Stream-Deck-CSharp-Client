using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using tech.aerove.streamdeck.client.Actions;

namespace tech.aerove.streamdeck.client
{
    public static class IServiceCollectionExtensions
    {
        public static void AddAeroveStreamDeckClient(this IServiceCollection services)
        {
            services.AddSingleton<StreamDeckInfo>();
            services.AddSingleton<ElgatoEventHandler>();
            services.AddTransient<ManifestInfo>();
            services.AddHostedService<WebSocketService>();
            services.AddSingleton<IActionFactory, DefaultActionFactory>();
            services.AddSingleton<IActionExecuter,DefaultActionExecuter>();
            //services.TryAddSingleton<IActionExecuter, DefaultActionExecuter>();

        }


    }
}