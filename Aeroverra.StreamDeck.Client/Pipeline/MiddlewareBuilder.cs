using Aeroverra.StreamDeck.Client.Events;
using Aeroverra.StreamDeck.Client.Pipeline.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Aeroverra.StreamDeck.Client.Pipeline
{
	public class MiddlewareBuilder : IAsyncDisposable, IDisposable
	{
		private readonly IServiceProvider _services;
		private readonly ILogger<MiddlewareBuilder> _logger;
		private readonly List<MiddlewareLifetime> _middlewareLifetimes = new List<MiddlewareLifetime>();
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
			var constructors = type
				.GetConstructors()
				.OrderByDescending(x => x.GetParameters().Count())
				.ToList();

			IServiceScope? chosenScope = null;
			object[]? parameterValues = null;

			foreach (var constructor in constructors)
			{
				var scope = _services.CreateScope();
				try
				{
					var resolvedParameters = new List<object>();
					foreach (var parameter in constructor.GetParameters())
					{
						var service = scope.ServiceProvider.GetService(parameter.ParameterType);
						if (service == null)
						{
							resolvedParameters.Clear();
							break;
						}
						resolvedParameters.Add(service);
					}

					if (resolvedParameters.Count > 0)
					{
						chosenScope = scope;
						parameterValues = resolvedParameters.ToArray();
						break;
					}
				}
				finally
				{
					if (chosenScope != scope)
					{
						scope.Dispose();
					}
				}
			}

			if (chosenScope == null || parameterValues == null)
			{
				_logger.LogError("Failed to create middleware instance of type {TypeName} because no resolvable constructor was found.", type.FullName);
				throw new Exception($"Failed to create middleware pipeline due to instance creation issue for {type.FullName}.");
			}

			MiddlewareBase? instance = null;
			try
			{
				instance = Activator.CreateInstance(type, parameterValues) as MiddlewareBase;
			}
			catch
			{
				chosenScope.Dispose();
				throw;
			}

			if (instance is null)
			{
				_logger.LogError("Failed to create middleware instance of type {TypeName}", type.FullName);
				throw new Exception($"Failed to create middleware pipeline due to instance creation issue.");
			}

			instance.NextDelegate = nextDelegate;
			_middlewareLifetimes.Add(new MiddlewareLifetime(instance, chosenScope));
			return instance;
		}

		public async ValueTask DisposeAsync()
		{
			foreach (var lifetime in _middlewareLifetimes)
			{
				try
				{
					await lifetime.DisposeAsync();
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Failed to dispose middleware {MiddlewareType}.", lifetime.Middleware.GetType().FullName);
				}
			}
			_middlewareLifetimes.Clear();
		}

		public void Dispose()
		{
			DisposeAsync().AsTask().GetAwaiter().GetResult();
		}
	}
}
