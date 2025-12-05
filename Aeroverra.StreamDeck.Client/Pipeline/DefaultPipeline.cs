using Aeroverra.StreamDeck.Client.Actions;
using Aeroverra.StreamDeck.Client.Cache;
using Aeroverra.StreamDeck.Client.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace Aeroverra.StreamDeck.Client.Pipeline
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
        private readonly IElgatoWebSocket _elgatoWebSocket;

        public DefaultPipeline(ILogger<DefaultPipeline> logger, MessageParser messageParser, IActionFactory actionFactory, IActionExecuter actionExecuter, ICache cache, MiddlewareBuilder middlewareBuilder, EventManager eventManager, IElgatoWebSocket elgatoWebSocket)
        {
            _logger = logger;
            _messageParser = messageParser;
            _actionFactory = actionFactory;
            _actionExecuter = actionExecuter;
            _cache = cache;
            _nextDelegate = middlewareBuilder.Build(HandleIncomingFinal, HandleOutgoingFinal);
            _eventManager = eventManager;
            _elgatoWebSocket = elgatoWebSocket;
        }

        public async Task StartListening(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    var message = await _elgatoWebSocket.ListenAsync();
                    if (message == null)
                    {
                        _logger.LogError("Recieved null message from {ClassName}", nameof(IElgatoWebSocket));
                        continue;
                    }

                    var elgatoEvent = _messageParser.Parse(message);
                    if (elgatoEvent == null)
                    {
                        _logger.LogError("Event parsed null from {ClassName} {Message}", nameof(IElgatoWebSocket), message);
                        continue;
                    }

                    await _nextDelegate.InvokeNextIncoming(elgatoEvent);

                }
                catch (WebSocketException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error when handling event");
                    await Task.Delay(1000);
                }

            }
        }

        public async Task HandleIncoming(string message)
        {
            var elgatoEvent = _messageParser.Parse(message);
            if (elgatoEvent == null) { return; }

            await _nextDelegate.InvokeNextIncoming(elgatoEvent);
        }

        public async Task HandleOutgoing(object message)
        {
            //todo: check if this is needed or not because it may just be doing the same thing the elgatowebsocket does
            var settings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() } };
            var json = JsonConvert.SerializeObject(message, settings);
            await _nextDelegate.InvokeNextOutgoing(JObject.Parse(json));
        }

        private async Task HandleIncomingFinal(IElgatoEvent elgatoEvent)
        {
            _cache.Update(elgatoEvent);
            var actions = _actionFactory.CreateActions(elgatoEvent);
            await _actionExecuter.ExecuteAsync(elgatoEvent, actions);
            _eventManager.HandleIncoming(elgatoEvent);
        }

        private Task HandleOutgoingFinal(JObject message)
        {
            return _elgatoWebSocket.SendAsync(message);
        }
    }
}
