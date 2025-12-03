using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Aeroverra.StreamDeck.Client.Events;
using Aeroverra.StreamDeck.Client.Pipeline.Middleware;

namespace Aeroverra.StreamDeck.Client.Pipeline
{
    public class MiddlewareBuilder
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<MiddlewareBuilder> _logger;
        public MiddlewareBuilder(IServiceProvider services, ILogger<MiddlewareBuilder> logger)
        {
            _services = services;
            _logger = logger;
        }
        private List<Type> Types = new List<Type>();
        public MiddlewareBuilder Add(Type type)
        {
            if (type != null && !Types.Contains(type) && type.IsSubclassOf(typeof(MiddlewareBase)))
            {
                Types.Add(type);
            }
            return this;
        }
        public MiddlewareBuilder Add<T>() where T : MiddlewareBase
        {
            var type = typeof(T);
            if (type != null && !Types.Contains(type))
            {
                Types.Add(type);
            }
            return this;
        }

        internal NextDelegate Build(Func<IElgatoEvent, Task> finalIncoming, Func<JObject, Task> finalOutgoing)
        {
            var nextDelegate = new NextDelegate(finalIncoming, finalOutgoing);
            var TypeList = Types.ToList();
            TypeList.Reverse();
            foreach (var type in TypeList)
            {
                var instance = GetInstance(type, nextDelegate);
                nextDelegate = new NextDelegate(instance.HandleIncoming, instance.HandleOutgoing);
            }
            return nextDelegate;
        }

        private MiddlewareBase GetInstance(Type type, NextDelegate nextDelegate)
        {
            var constructors = type.GetConstructors()
                .OrderByDescending(x => x.GetParameters().Count())
                .ToList();

            List<object> parameters = new List<object>();
            foreach (var constructor in constructors)
            {
                using (var scope = _services.CreateScope())
                {
                    foreach (var parameter in constructor.GetParameters())
                    {
                        var service = scope.ServiceProvider.GetService(parameter.ParameterType);
                        if (service == null)
                        {
                            parameters.Clear();
                            break;
                        }
                        parameters.Add(service);
                    }
                    if (parameters.Count > 0)
                    {
                        break;
                    }
                }
            }
            var instance = Activator.CreateInstance(type, parameters.ToArray()) as MiddlewareBase;
            instance.NextDelegate = nextDelegate;
            return instance;
        }
    }
}
