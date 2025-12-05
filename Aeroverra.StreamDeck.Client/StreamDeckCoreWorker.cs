using Aeroverra.StreamDeck.Client.Pipeline;
using Aeroverra.StreamDeck.Client.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;

namespace Aeroverra.StreamDeck.Client
{
    internal class StreamDeckCoreWorker(ILogger<StreamDeckCoreWorker> logger, IPipeline pipeline, IElgatoWebSocket elgatoWebSocket, SettingsListenerService settingsListenerService) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await settingsListenerService.StartListeningAsync();
                await elgatoWebSocket.ConnectAsync();
                Task listenerTask = ListenerTask(stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
                await elgatoWebSocket.DisconnectAsync();
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                logger.LogCritical(e, "websocketservice error");
            }

            logger.LogInformation("{ClientName} Shutting Down...", "Aerove Stream Deck Client");
        }

        private async Task ListenerTask(CancellationToken cancellationToken)
        {
            try
            {
                await pipeline.StartListening(cancellationToken);
            }
            catch (WebSocketException)
            {
                logger.LogCritical("Websocket died, shutting down.");
                Environment.Exit(0);
            }
        }
    }
}
