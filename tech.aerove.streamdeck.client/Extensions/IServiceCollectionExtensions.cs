using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace tech.aerove.streamdeck.client
{
    public static class IServiceCollectionExtensions
    {
        public static void AddAeroveStreamDeckClient(this IServiceCollection services)
        {
            services.AddHostedService<WebSocketService>();
        }


    }
}