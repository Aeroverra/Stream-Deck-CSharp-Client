using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tech.Aerove.StreamDeck.Client.Actions;
using Tech.Aerove.StreamDeck.Client.Cache;
using Tech.Aerove.StreamDeck.Client.Events;
using Tech.Aerove.StreamDeck.Client.Pipeline;
using Tech.Aerove.StreamDeck.Client.Pipeline.Middleware;
using Tech.Aerove.StreamDeck.Client.SDAnalyzer;
using Tech.Aerove.StreamDeck.Client.Startup;

namespace Tech.Aerove.StreamDeck.Client
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
        public static IServiceCollection AddAeroveStreamDeckClient(this IServiceCollection services, HostBuilderContext context)
        {
            Console.Title = "Aerove Stream Deck Client by Aeroverra";
            var config = context.Configuration;
            TemplateUpdater.UpdateTemplate(config);
            string[] args = Environment.GetCommandLineArgs();
            VSDebugHandler.OutputArgs(config);
            args = VSDebugHandler.DevDebug(config) ?? args;


            services.AddSingleton<StreamDeckInfo>(x => new StreamDeckInfo(x.GetRequiredService<ILogger<StreamDeckInfo>>(), args.ToList()));
            services.AddTransient<ManifestInfo>();
            services.AddSingleton<WebSocketService>();
            services.AddHostedService<WebSocketService>(provider => provider.GetService<WebSocketService>());

            services.AddSingleton<IPipeline, DefaultPipeline>();
            services.AddSingleton<MessageParser>();
            services.AddSingleton<ICache, DefaultCache>();
            services.AddSingleton<IActionFactory, DefaultActionFactory>();
            //services.AddSingleton<IActionFactory, DefaultTransientActionFactory>();
            services.AddSingleton<IActionExecuter, DefaultActionExecuter>();
            services.AddSingleton<EventManager>();
            services.AddSingleton<IElgatoDispatcher, DefaultElgatoDispatcher>();


            if (config.GetValue<bool>("SDAnalyzerEnabled", true))
            {
                //services.AddSingleton<SDAnalyzerService>();
                //services.AddMiddleware<SDAnalyzerMiddleware>();
            }


            services.AddSingleton(x =>
            {
                var builder = new MiddlewareBuilder(x, x.GetService<ILogger<MiddlewareBuilder>>());
                if (config.GetValue<bool>("ElgatoEventLogging", true))
                {
                    services.AddMiddleware<EventLoggingMiddleware>();
                }
                services.AddMiddleware<AeroveMiddleware>();
                services.AddMiddleware<EventOrderingMiddleware>();
                MiddlewareTypes.ForEach(x => builder.Add(x));

                return builder;
            });

            return services;


        }
        private static List<Type> MiddlewareTypes = new List<Type>();
        public static IServiceCollection AddMiddleware<T>(this IServiceCollection services) where T : MiddlewareBase
        {
            MiddlewareTypes.Add(typeof(T));
            return services;
        }

    }
}