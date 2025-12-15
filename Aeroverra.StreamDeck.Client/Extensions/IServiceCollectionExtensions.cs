using Aeroverra.StreamDeck.Client.Actions;
using Aeroverra.StreamDeck.Client.Cache;
using Aeroverra.StreamDeck.Client.Events;
using Aeroverra.StreamDeck.Client.Pipeline;
using Aeroverra.StreamDeck.Client.Pipeline.Middleware;
using Aeroverra.StreamDeck.Client.Services;
using Aeroverra.StreamDeck.Client.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Aeroverra.StreamDeck.Client
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
            ILogger logger = NullLoggerFactory.Instance.CreateLogger("IServiceCollectionExtensions");

            Console.Title = "Aerove Stream Deck Client by Aeroverra";
            string[] args = Environment.GetCommandLineArgs();
            var config = context.Configuration;
            var devDebug = config.GetValue<bool>("DevDebug");
            var logParametersOnly = config.GetValue<bool>("DevLogParametersOnly");

            TemplateUpdater.UpdateTemplate(config);

            if (logParametersOnly == true)
            {
                DevDebug.OutputArgs(config);
                logger.LogInformation("WebSocketService did not startup. Logging parameters only.");
            }
            else if (devDebug == true)
            {
                args = DevDebug.TakeOver(config);
            }
            if (logParametersOnly == false)
            {
                services.AddHostedService<StreamDeckCoreWorker>();
            }
            services.AddSingleton<IElgatoWebSocket, ElgatoWebSocket>();
            services.AddSingleton<StreamDeckInfo>(x => new StreamDeckInfo(x.GetRequiredService<ILogger<StreamDeckInfo>>(), args.ToList()));
            services.AddSingleton<IPipeline, DefaultPipeline>();

            services.AddTransient<ManifestInfo>();




            services.AddSingleton<MessageParser>();
            services.AddSingleton<ICache, DefaultCache>();
            services.AddSingleton<IActionFactory, DefaultActionFactory>();
            //services.AddSingleton<IActionFactory, DefaultTransientActionFactory>();
            services.AddSingleton<IActionExecuter, DefaultActionExecuter>();
            services.AddSingleton<EventManager>();
            services.AddSingleton<IElgatoDispatcher, DefaultElgatoDispatcher>();
            services.AddSingleton<SettingsListenerService>();

            if (config.GetValue<bool>("SDAnalyzerEnabled", true))
            {
                //services.AddSingleton<SDAnalyzerService>();
                //services.AddMiddleware<SDAnalyzerMiddleware>();
            }


            services.AddSingleton(x =>
            {
                var builder = new MiddlewareBuilder(x, x.GetService<ILogger<MiddlewareBuilder>>());
                services.AddMiddleware<IdSDKMiddleware>();
                if (config.GetValue<bool>("ElgatoEventLogging", true))
                {
                    services.AddMiddleware<EventLoggingMiddleware>();
                }
                services.AddMiddleware<AeroverraMiddleware>();
                services.AddMiddleware<EventOrderingMiddleware>();
                services.AddMiddleware<LifeCycleMiddleware>();


                if (config.GetValue<bool>("ElgatoEventLogging", true))
                {
                    services.AddMiddleware<SDKEventLoggingMiddleware>();
                }
                MiddlewareTypes.ForEach(x => builder.Add(x));

                return builder;
            });

            services.TryAddSingleton<IGlobalSettings, DefaultGlobalSettings>();
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