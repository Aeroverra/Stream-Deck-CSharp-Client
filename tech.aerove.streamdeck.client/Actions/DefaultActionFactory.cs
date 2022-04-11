using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Events;

namespace tech.aerove.streamdeck.client.Actions
{
    internal class DefaultActionFactory : IActionFactory
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<DefaultActionFactory> _logger;
        public DefaultActionFactory(IServiceProvider services, ILogger<DefaultActionFactory> logger)
        {
            _services = services;
            _logger = logger;
        }
        public List<ActionBase> CreateActions(ElgatoEvent elgatoEvent)
        {
            var types = GetTypes(elgatoEvent);

            List<ActionBase> actions = new List<ActionBase>();
            foreach (var type in types)
            {
                var constructor = GetConstructor(type);
                if (constructor == null)
                {
                    _logger.LogWarning("Could not find valid constructor for type: {type}", type);
                    continue;
                }
                ActionBase action = GetInstance(type, constructor);
                actions.Add(action);
            }
            return actions;
        }
        private List<Type> GetTypes(ElgatoEvent elgatoEvent)
        {
            var types = Assembly.GetEntryAssembly()
               .GetTypes()
               .Where(x => x.IsClass)
               .Where(x => x.IsSubclassOf(typeof(ActionBase)))
               .ToList();

            if (elgatoEvent is IActionEvent)
            {
                var actionId = (elgatoEvent as IActionEvent).Action.ToLower();
                var attTypes = types
                    .Where(x => x.GetCustomAttributes(typeof(PluginAction), true).Length > 0)
                    .ToList();
                attTypes = attTypes
                    .Where(x => (x.GetCustomAttributes(typeof(PluginAction), true).First() as PluginAction).Id.ToLower() == actionId)
                    .ToList();
                if (attTypes.Count == 0)
                {
                    var actionName = actionId.Split(".").Last();
                    attTypes = types
                        .Where(x => x.Name.ToLower() == actionName)
                        .ToList();
                    types = attTypes;
                }
                if (types.Count > 1)
                {
                    _logger.LogWarning("Multiple actions were found for {actionId}. Consider using the PluginAction Attribute!", actionId);
                }
                if (types.Count == 0)
                {
                    _logger.LogWarning("No Actions were found for {actionId}!", actionId);

                }

            }
            return types;
        }
        private ActionBase GetInstance(Type type, ConstructorInfo constructorInfo)
        {
            List<object> parameters = new List<object>();
            foreach (var parameter in constructorInfo.GetParameters())
            {
                var service = _services.GetService(type);
                parameters.Add(service);
            }
            ActionBase action = Activator.CreateInstance(type, parameters.ToArray()) as ActionBase;
            return action;

        }
        private ConstructorInfo? GetConstructor(Type type)
        {
            List<ConstructorInfo> validConstructors = new List<ConstructorInfo>();
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                var resolvable = true;
                foreach (var parameter in parameters)
                {
                    var service = _services.GetService(type);
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
            if (validConstructors.Count == 0)
            {
                return null;
            }
            return validConstructors.OrderByDescending(x => x.GetParameters()).First();
        }
    }
}
