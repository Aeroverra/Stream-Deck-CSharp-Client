using Aeroverra.StreamDeck.Client.Cache;
using Aeroverra.StreamDeck.Client.Events;
using Aeroverra.StreamDeck.Client.Events.SDKEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Aeroverra.StreamDeck.Client.Actions
{
	internal class ActionInfo
	{
		public required string UUID { get; set; }
		public required Type Type { get; set; }
		public required ConstructorInfo ConstructorInfo { get; set; }
		public required List<PropertyInfo> InjectedProperties { get; set; }
	}

	internal class DefaultActionFactory : IActionFactory, IAsyncDisposable
	{
		private readonly IServiceProvider _services;
		private readonly ILogger<DefaultActionFactory> _logger;
		private readonly ICache _cache;
		private readonly ManifestInfo _manifest;
		private readonly IElgatoDispatcher _dispatcher;
		private List<ActionInfo> ActionInfo = new List<ActionInfo>();
		private Dictionary<string, ActionLifetime> Instances = new Dictionary<string, ActionLifetime>();
        public DefaultActionFactory(IServiceProvider services, ILogger<DefaultActionFactory> logger,
            ICache cache, ManifestInfo manifest, IElgatoDispatcher dispatcher)
        {
            _services = services;
            _logger = logger;
            _cache = cache;
            _manifest = manifest;
            _dispatcher = dispatcher;
            InitializeAndCacheKnownActionTypes();
        }


		public List<ActionBase> CreateActions(IElgatoEvent elgatoEvent)
		{
			// Get all instances we need to notify for this event
			var instanceIds = GetInstanceIdsToNotify(elgatoEvent);
			List<ActionBase> actions = new List<ActionBase>();

			lock (Instances)
			{
				foreach (var instanceId in instanceIds)
				{
					if (Instances.TryGetValue(instanceId, out ActionLifetime? lifetime))
					{
						actions.Add(lifetime.Action);
						continue;
					}

					// Instance does not exist yet so create it and keep the scope alive for its lifetime.
					var newLifetime = CreateActionLifetime(instanceId);
					if (newLifetime != null)
					{
						actions.Add(newLifetime.Action);
						Instances.Add(instanceId, newLifetime);
					}
				}
			}

			return actions;
		}

		public async ValueTask CleanupAsync(IElgatoEvent elgatoEvent, IReadOnlyCollection<ActionBase> actions)
		{
			if (actions.Count == 0)
			{
				return;
			}

			// Only dispose owned scopes when the action lifecycle has ended.
			if (elgatoEvent is not DisposeEvent)
			{
				return;
			}

			List<ActionLifetime> lifetimesToDispose = new List<ActionLifetime>();

			lock (Instances)
			{
				foreach (var action in actions)
				{
					if (Instances.TryGetValue(action.Context.InstanceId, out var lifetime))
					{
						Instances.Remove(action.Context.InstanceId);
						lifetimesToDispose.Add(lifetime);
					}
				}

				if (lifetimesToDispose.Count != actions.Count)
				{
					_logger.LogWarning("Dispose event targeted {ActionCount} action(s) but only {LifetimeCount} lifetime(s) were tracked.", actions.Count, lifetimesToDispose.Count);
				}
			}

			foreach (var lifetime in lifetimesToDispose)
			{
				try
				{
					await lifetime.DisposeAsync();
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Failed to dispose scoped action instance {InstanceId}.", lifetime.InstanceId);
				}
			}
		}

		/// <summary>
		/// Creates an instance based off the action instance id and keeps the DI scope alive for as long as the action exists.
		/// </summary>
		/// <param name="instanceId"></param>
		/// <returns></returns>
		private ActionLifetime? CreateActionLifetime(string instanceId)
		{
			IActionContext actionContext = _cache.BuildContext(instanceId);
			IActionDispatcher actionDispatcher = new DefaultActionDispatcher(_dispatcher, actionContext);
			var actionInfo = ActionInfo.FirstOrDefault(x => x.UUID == actionContext.ActionUUID);

			//custructor not known or action class not known
			if (actionInfo == null || actionInfo.ConstructorInfo == null)
			{
				return null;
			}

			var scope = _services.CreateScope();

			try
			{
				//IServiceProvider.GetService
				List<object> parameters = new List<object>();
				foreach (var parameter in actionInfo.ConstructorInfo.GetParameters())
				{
                    var service = scope.ServiceProvider.GetRequiredService(parameter.ParameterType);
					parameters.Add(service);
				}


				ActionBase action = (Activator.CreateInstance(actionInfo.Type, parameters.ToArray()) as ActionBase)!;

                foreach (var property in actionInfo.InjectedProperties)
                {
                    var service = scope.ServiceProvider.GetRequiredService(property.PropertyType);
                    property.SetValue(action, service);
				}

				action.Dispatcher = actionDispatcher;
				action.Context = actionContext;

				return new ActionLifetime(action, scope, actionContext.InstanceId);
			}
			catch
			{
				scope.Dispose();
				throw;
			}
		}

        /// <summary>
        /// Gets instance ids for the specified event. if the event has a specified context
        /// than only a single instance will be returned. If the event is a generic
        /// event like "devicedidconnect" than all known instance ids will be returned
        /// </summary>
        /// <param name="elgatoEvent"></param>
        /// <returns></returns>
        private List<string> GetInstanceIdsToNotify(IElgatoEvent elgatoEvent)
        {
            switch (elgatoEvent.Event)
            {
                //global events without an instance specified
                case ElgatoEventType.DidReceiveGlobalSettings:
                case ElgatoEventType.DeviceDidConnect:
                case ElgatoEventType.DeviceDidDisconnect:
                case ElgatoEventType.ApplicationDidLaunch:
                case ElgatoEventType.ApplicationDidTerminate:
                case ElgatoEventType.SystemDidWakeUp:
                    return _cache.GetAllInstanceIds();
            }
            return new List<string>()
            {
                (elgatoEvent as IActionEvent)!.Context
            };

        }

        /// <summary>
        /// Pulls all the UUIDs from the manifest and initilizes the ActionInfo with the
        /// corresponding type information
        /// </summary>
        private void InitializeAndCacheKnownActionTypes()
        {
            //get all types that inherit actionbase
            var types = Assembly.GetEntryAssembly()
                .GetTypes()
                .Where(x => x.IsClass)
                .Where(x => x.IsSubclassOf(typeof(ActionBase)))
                .ToList();

            //grab all uuids from manifest
            var uuids = _manifest.Actions
                .Select(x => x.Uuid)
                .ToList();

            //add each uuid to dictionary with type
            foreach (var uuid in uuids)
            {
                var attTypes = types
                    .Where(x => x.GetCustomAttributes(typeof(PluginActionAttribute), true).Length > 0)
                    .Where(x => (x.GetCustomAttributes(typeof(PluginActionAttribute), true).First() as PluginActionAttribute)!.Id.ToLower() == uuid)
                    .ToList();

                //must not have an attribute, try filtering by name
                if (attTypes.Count == 0)
                {
                    var actionName = uuid.Split(".").Last();
                    attTypes = types
                        .Where(x => x.Name.ToLower() == actionName.ToLower())
                        .ToList();
                }
                if (attTypes.Count > 1)
                {
                    _logger.LogWarning("Multiple actions were found for {UUID}. Consider using the PluginAction Attribute! Using {Type}", uuid, attTypes.First());
                }
                if (attTypes.Count == 0)
                {
                    _logger.LogWarning("No Actions were found for {UUID} and events will not be fired for this action. Consider using the PluginAction Attribute!", uuid);
                    continue;
                }
                var type = attTypes.First();
                ActionInfo.Add(new ActionInfo
                {
                    UUID = uuid,
                    Type = type,
                    ConstructorInfo = GetConstructor(type)!,
                    InjectedProperties = GetInjectedProperties(type)
                });

            }

        }


        /// <summary>
        /// Returns the constructor with the highest number of resolvable services
        /// from the service container
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ConstructorInfo? GetConstructor(Type type)
        {
            List<ConstructorInfo> validConstructors = new List<ConstructorInfo>();
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                using (var scope = _services.CreateScope())
                {
                    var parameters = constructor.GetParameters();
                    var resolvable = true;
                    foreach (var parameter in parameters)
                    {
                        var service = scope.ServiceProvider.GetService(parameter.ParameterType);

                        if (service == null)
                        {
                            resolvable = false;
                            break;
                        }
                    }
                    if (resolvable)
                    {
                        validConstructors.Add(constructor);
                    }
                }
            }
            if (validConstructors.Count == 0)
            {
                _logger.LogWarning("Could not find a resolvable constructor for action type {Type}. No actions will be fired. ", type);
                return null;
            }
            return validConstructors.OrderByDescending(x => x.GetParameters()).First();
        }

		private List<PropertyInfo> GetInjectedProperties(Type type)
		{
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(p => p.GetCustomAttribute<InjectAttribute>() != null)
				.ToList();
			return properties;
		}

		/// <summary>
		/// Ensures any outstanding scoped action lifetimes are cleaned up if the factory itself is disposed.
		/// </summary>
		public async ValueTask DisposeAsync()
		{
			List<ActionLifetime> lifetimes;

			lock (Instances)
			{
				lifetimes = Instances.Values.ToList();
				Instances.Clear();
			}

			foreach (var lifetime in lifetimes)
			{
				try
				{
					await lifetime.DisposeAsync();
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Failed to dispose scoped action instance {InstanceId} during factory shutdown.", lifetime.InstanceId);
				}
			}
		}

	}
}
