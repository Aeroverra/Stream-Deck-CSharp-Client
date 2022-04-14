using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Actions;
using tech.aerove.streamdeck.client.Cache;
using tech.aerove.streamdeck.client.Events;

namespace tech.aerove.streamdeck.client
{
    public class ElgatoEventHandler
    {
        private readonly ILogger<ElgatoEventHandler> _logger;
        private readonly StreamDeckInfo _streamDeckInfo;
        private readonly IActionFactory _factory;
        private readonly IActionExecuter _executer;
        private readonly ManifestInfo _manifest;
        private Data Data { get; set; } = new Data();
        public ElgatoEventHandler(ILogger<ElgatoEventHandler> logger, StreamDeckInfo streamDeckInfo, IActionFactory factory, IActionExecuter executer, ManifestInfo manifest)
        {
            _logger = logger;
            _streamDeckInfo = streamDeckInfo;
            _factory = factory;
            _executer = executer;
            _manifest = manifest;
        }

        public async Task HandleIncoming(string message)
        {
            var elgatoEvent = ElgatoEvent.FromJson(message);
            _logger.LogDebug("Received: {eventtype}", elgatoEvent.Event);
            _logger.LogDebug("{resultString}", message);
            if (elgatoEvent == null)
            {
                _logger.LogWarning("Could not parse event: {resultString}", message);
                return;
            }
            switch (elgatoEvent.Event)
            {
                case ElgatoEventType.DeviceDidConnect:
                    DeviceDidConnect(elgatoEvent);
                        break;
                case ElgatoEventType.WillAppear:
                    DeviceDidConnect(elgatoEvent);
                    break;

            }
            var actions = _factory.CreateActions(elgatoEvent);
            await _executer.ExecuteAsync(elgatoEvent, actions);
        }
        private void DeviceDidConnect(DeviceDidConnectEvent e)
        {
            if(Data.Devices)
        }
    }
}
