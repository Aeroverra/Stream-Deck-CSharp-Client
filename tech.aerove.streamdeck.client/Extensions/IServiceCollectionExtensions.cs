using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using tech.aerove.streamdeck.client.Actions;
using tech.aerove.streamdeck.client.Cache;
using tech.aerove.streamdeck.client.Events;
using tech.aerove.streamdeck.client.Pipeline;
using tech.aerove.streamdeck.client.Pipeline.Middleware;
using tech.aerove.streamdeck.client.Startup;

namespace tech.aerove.streamdeck.client
{
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds the Aerove Stream Deck Client to your project.
        /// This handles are the hard work of communicating with the stream deck
        /// while providing a lot of QOL features making plugin development easy.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="context"></param>
        /// <param name="args"></param>
        public static IServiceCollection AddAeroveStreamDeckClient(this IServiceCollection services, HostBuilderContext context, string[] args)
        {
            var config = context.Configuration;

            VSDebugHandler.OutputArgs(config, args);
            args = VSDebugHandler.DevDebug(config) ?? args;

            services.AddSingleton<StreamDeckInfo>(x => new StreamDeckInfo(x.GetRequiredService<ILogger<StreamDeckInfo>>(), args.ToList()));
            services.AddTransient<ManifestInfo>();
            services.AddSingleton<WebSocketService>();
            services.AddHostedService<WebSocketService>(provider => provider.GetService<WebSocketService>());

            services.AddSingleton<IPipeline, DefaultPipeline>();
            services.AddSingleton<MessageParser>();
            services.AddSingleton<ICache, DefaultCache>();
            services.AddSingleton<IActionFactory, DefaultActionFactory>();
            services.AddSingleton<IActionExecuter, DefaultActionExecuter>();
            services.AddSingleton<EventManager>();
            services.AddSingleton<IElgatoDispatcher, DefaultElgatoDispatcher>();

            services.AddMiddleware<EventLoggingMiddleware>();
            services.AddMiddleware<EventOrderingMiddleware>();
       
 
         
            services.AddSingleton(x =>
            {
                var builder = new MiddlewareBuilder(x, x.GetService<ILogger<MiddlewareBuilder>>());
                MiddlewareTypes.ForEach(x => builder.Add(x));
                return builder;
            });
   
            return services;


        }
        private static List<Type> MiddlewareTypes = new List<Type>();
        private static IServiceCollection AddMiddleware<T>(this IServiceCollection services) where T : MiddlewareBase
        {
            MiddlewareTypes.Add(typeof(T));
            return services;
        }

    }
}