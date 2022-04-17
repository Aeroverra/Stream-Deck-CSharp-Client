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
        private readonly IElgatoDispatcher _dispatcher;
        private bool FirstEventHandled = false;
        private bool AwaitingInitialGlobalSettings = true;
        private List<string> PendingMessages = new List<string>();
        public DefaultElgatoEventHandler(ILogger<DefaultElgatoEventHandler> logger,
            IActionFactory factory, IActionExecuter executer,
            ICache cache, IElgatoDispatcher dispatcher)
        {
            _logger = logger;
            _factory = factory;
            _executer = executer;
            _cache = cache;
            _dispatcher = dispatcher;
        }

        public async Task HandleIncoming(string message)
        {
            if (!FirstEventHandled)
            {
                FirstEventHandled = true;
                await _dispatcher.GetGlobalSettingsAsync();
            }

            _logger.LogDebug("{resultString}", message);
            var elgatoEvent = ElgatoEvent.FromJson(message);
            if (elgatoEvent == null)
            {
                _logger.LogWarning("Could not parse event: {resultString}", message);
                return;
            }
            if (AwaitingInitialGlobalSettings && elgatoEvent.Event != ElgatoEventType.DidReceiveGlobalSettings)
            {
                PendingMessages.Add(message);
                return;
            }

            _logger.LogDebug("Received: {eventtype}", elgatoEvent.Event);

            _cache.Update(elgatoEvent);
            var actions = _factory.CreateActions(elgatoEvent);
            await _executer.ExecuteAsync(elgatoEvent, actions);

            if (AwaitingInitialGlobalSettings)
            {
                AwaitingInitialGlobalSettings = false;
                foreach(var pendingmessage in PendingMessages)
                {
                    await HandleIncoming(pendingmessage);
                }
            }
        }


    }
}
