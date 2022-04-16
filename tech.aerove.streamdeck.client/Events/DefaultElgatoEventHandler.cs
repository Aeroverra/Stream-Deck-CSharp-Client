using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Actions;
using tech.aerove.streamdeck.client.Cache;
using tech.aerove.streamdeck.client.Startup;

namespace tech.aerove.streamdeck.client.Events
{
    internal class DefaultElgatoEventHandler : IElgatoEventHandler
    {
        private readonly ILogger<DefaultElgatoEventHandler> _logger;
        private readonly IActionFactory _factory;
        private readonly IActionExecuter _executer;
        private readonly ICache _cache;
        public DefaultElgatoEventHandler(ILogger<DefaultElgatoEventHandler> logger,
            IActionFactory factory, IActionExecuter executer,
            ICache cache)
        {
            _logger = logger;
            _factory = factory;
            _executer = executer;
            _cache = cache;
        }

        public async Task HandleIncoming(string message)
        {
            _logger.LogDebug("{resultString}", message);
            var elgatoEvent = ElgatoEvent.FromJson(message);
            if (elgatoEvent == null)
            {
                _logger.LogWarning("Could not parse event: {resultString}", message);
                return;
            }
            _logger.LogDebug("Received: {eventtype}", elgatoEvent.Event);

            _cache.Update(elgatoEvent);
            var actions = _factory.CreateActions(elgatoEvent);
            await _executer.ExecuteAsync(elgatoEvent, actions);
        }


    }
}
