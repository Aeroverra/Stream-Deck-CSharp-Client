using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Tech.Aerove.StreamDeck.Client.Actions;
using Tech.Aerove.StreamDeck.Client.Cache;
using Tech.Aerove.StreamDeck.Client.Events;

namespace Tech.Aerove.StreamDeck.Client.Pipeline
{
    internal class DefaultPipeline : IPipeline
    {
        private readonly ILogger<DefaultPipeline> _logger;
        private readonly MessageParser _messageParser;
        private readonly IActionFactory _actionFactory;
        private readonly IActionExecuter _actionExecuter;
        private readonly ICache _cache;
        private readonly NextDelegate _nextDelegate;
        private readonly EventManager _eventManager;
        private WebSocketService _socket;
        public DefaultPipeline(ILogger<DefaultPipeline> logger, MessageParser messageParser, IActionFactory actionFactory, IActionExecuter actionExecuter, ICache cache, MiddlewareBuilder middlewareBuilder, EventManager eventManager)
        {
            _logger = logger;
            _messageParser = messageParser;
            _actionFactory = actionFactory;
            _actionExecuter = actionExecuter;
            _cache = cache;
            _nextDelegate = middlewareBuilder.Build(HandleIncomingFinal, HandleOutgoingFinal);
            _eventManager = eventManager;

        }
        public void SetWebSocket(WebSocketService socket)
        {
            if (_socket == null)
            {
                _socket = socket;
            }
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
            var settings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() } };
            var json = JsonConvert.SerializeObject(message, settings);
            await _nextDelegate.InvokeNextOutgoing(JObject.Parse(json));
        }
        private Task HandleOutgoingFinal(JObject message)
        {
            return _socket.SendAsync(message);
        }


    }
}
