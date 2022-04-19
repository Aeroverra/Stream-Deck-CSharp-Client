using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Actions;
using tech.aerove.streamdeck.client.Cache;
using tech.aerove.streamdeck.client.Events;

namespace tech.aerove.streamdeck.client.Pipeline
{
    internal class DefaultPipeline : IPipeline
    {
        private readonly ILogger<DefaultPipeline> _logger;
        private readonly MessageParser _messageParser;
        private readonly IActionFactory _actionFactory;
        private readonly IActionExecuter _actionExecuter;
        private readonly ICache _cache;
        private readonly IServiceProvider _services;
        private readonly NextDelegate _nextDelegate;
        private readonly EventManager _eventManager;
        public DefaultPipeline(ILogger<DefaultPipeline> logger, MessageParser messageParser, IActionFactory actionFactory, IActionExecuter actionExecuter, ICache cache, IServiceProvider services, MiddlewareBuilder middlewareBuilder, EventManager eventManager)
        {
            _logger = logger;
            _messageParser = messageParser;
            _actionFactory = actionFactory;
            _actionExecuter = actionExecuter;
            _cache = cache;
            _services = services;
            _nextDelegate = middlewareBuilder.Build(HandleIncomingFinal, HandleOutgoingFinal);
            _eventManager = eventManager;
        }

        public async Task HandleIncoming(string message)
        {
            var elgatoEvent = _messageParser.Parse(message);
            if (elgatoEvent == null) { return; }

            await _nextDelegate.InvokeNextIncoming(elgatoEvent);
        }
        private async Task HandleIncomingFinal(IElgatoEvent elgatoEvent)
        {
            _cache.Update(elgatoEvent);
            var actions = _actionFactory.CreateActions(elgatoEvent);
            await _actionExecuter.ExecuteAsync(elgatoEvent, actions);
            _eventManager.HandleIncoming(elgatoEvent);
        }

        public async Task HandleOutgoing(object message)
        {

        }
        public async Task HandleOutgoingFinal(object message)
        {

        }
    }
}
