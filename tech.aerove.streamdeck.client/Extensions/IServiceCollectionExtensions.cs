using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using tech.aerove.streamdeck.client.Actions;
using tech.aerove.streamdeck.client.Models;

namespace tech.aerove.streamdeck.client
{
    public static class IServiceCollectionExtensions
    {
        public static void AddAeroveStreamDeckClient(this IServiceCollection services)
        {
            services.AddSingleton<StreamDeckInfo>();
            services.AddHostedService<WebSocketService>();
            services.AddSingleton<IActionFactory, DefaultActionFactory>();
            services.AddSingleton<IActionExecuter,DefaultActionExecuter>();

        }


    }
}