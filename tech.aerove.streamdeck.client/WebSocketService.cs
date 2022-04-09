using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Models;

namespace tech.aerove.streamdeck.client
{
    public class WebSocketService : BackgroundService
    {
        private readonly ILogger<WebSocketService> _logger;
        private StreamDeckInfo StreamDeckInfo = new StreamDeckInfo();

        public WebSocketService(ILogger<WebSocketService> logger, IConfiguration config)
        {
            _logger = logger;
            config.Bind(StreamDeckInfo);
            _logger.LogInformation(StreamDeckInfo.DeviceInfo.Application.Platform);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(25000, stoppingToken);
            }
        }
    }
}
