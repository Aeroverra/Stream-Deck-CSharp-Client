using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tech.Aerove.StreamDeck.Client.Cache;
using Tech.Aerove.StreamDeck.Client.Events;
using Tech.Aerove.StreamDeck.Client.Startup;

namespace Tech.Aerove.StreamDeck.Client.Actions
{
    internal class ActionInfo
    {
        public string UUID { get; set; }
        public Type Type { get; set; }
        public ConstructorInfo ConstructorInfo { get; set; }
    }
    internal class DefaultActionFactory : IActionFactory
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<DefaultTransientActionFactory> _logger;
        private readonly ICache _cache;
        private readonly ManifestInfo _manifest;
        private readonly IElgatoDispatcher _dispatcher;
        private List<ActionInfo> ActionInfo = new List<ActionInfo>();
        private Dictionary<string, ActionBase> Instances = new Dictionary<string, ActionBase>();
        public DefaultActionFactory(IServiceProvider services, ILogger<DefaultTransientActionFactory> logger,
            ICache cache, ManifestInfo manifest, IElgatoDispatcher dispatcher)
        {
            _services = services;
            _logger = logger;
            _cache = cache;
            _manifest = manifest;
            _dispatcher = dispatcher;
            InitializeTypes();

        }


        public List<ActionBase> CreateActions(IElgatoEvent elgatoEvent)
        {
            lock (Instances)
            {
                var instanceIds = GetInstanceIds(elgatoEvent);
                List<ActionBase> actions = new List<ActionBase>();
                var existingKeys = new List<string>();
                foreach (var instanceId in instanceIds)
                {
                    var exists = Instances.Any(x => x.Key == instanceId);
                    if (!exists) { continue; }
                    var instance = Instances.Where(x => x.Key == instanceId).Select(x => x.Value).SingleOrDefault();
                    existingKeys.Add(instanceId);
                    actions.Add(instance);
                }
                instanceIds.RemoveAll(x => existingKeys.Contains(x));
                foreach (var instanceId in instanceIds)
                {
                    ActionBase action = GetInstance(instanceId);
                    if (action != null)
                    {
                        actions.Add(action);
                        Instances.Add(instanceId, action);
                    }
                }
                return actions;
            }
        
        }

        /// <summary>
        /// Creates an instance based off the action instance id
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        private ActionBase? GetInstance(string instanceId)
        {
            IActionContext actionContext = _cache.BuildContext(instanceId);
            IActionDispatcher actionDispatcher = new DefaultActionDispatcher(_dispatcher, actionContext);
            var actionInfo = ActionInfo.FirstOrDefault(x => x.UUID == actionContext.ActionUUID);

            //custructor not known or action class not known
            if (actionInfo == null || actionInfo.ConstructorInfo == null)
            {
                return null;
            }

            using (var scope = _services.CreateScope())
            {
                //IServiceProvider.GetService
                List<object> parameters = new List<object>();
                foreach (var parameter in actionInfo.ConstructorInfo.GetParameters())
                {
                    var service = scope.ServiceProvider.GetRequiredService(parameter.ParameterType);
                    parameters.Add(service);
                }


                ActionBase action = Activator.CreateInstance(actionInfo.Type, parameters.ToArray()) as ActionBase;

                action.Dispatcher = actionDispatcher;
                action.Context = actionContext;
                return action;
            }
        }

        /// <summary>
        /// Gets instance ids for the specified event. if the event has a specified context
        /// than only a single instance will be returned. If the event is a generic
        /// event like "devicedidconnect" than all known instance ids will be returned
        /// </summary>
        /// <param name="elgatoEvent"></param>
        /// <returns></returns>
        private List<string> GetInstanceIds(IElgatoEvent elgatoEvent)
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
                (elgatoEvent as IActionEvent).Context
            };

        }

        /// <summary>
        /// Pulls all the UUIDs from the manifest and initilizes the ActionInfo with the
        /// corresponding type information
        /// </summary>
        private void InitializeTypes()
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
                    .Where(x => x.GetCustomAttributes(typeof(PluginAction), true).Length > 0)
                    .Where(x => (x.GetCustomAttributes(typeof(PluginAction), true).First() as PluginAction).Id.ToLower() == uuid)
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
                    ConstructorInfo = GetConstructor(type)
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

    }
}
