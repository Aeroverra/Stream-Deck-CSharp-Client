using Aeroverra.StreamDeck.Client.Pipeline;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aeroverra.StreamDeck.Client
{
    public class StreamDeckCoreWorker(ILogger<StreamDeckCoreWorker> logger, IPipeline pipeline, IElgatoWebSocket elgatoWebSocket) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await elgatoWebSocket.ConnectAsync();
                Task listenerTask = pipeline.StartListening(stoppingToken);

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

    }
}
