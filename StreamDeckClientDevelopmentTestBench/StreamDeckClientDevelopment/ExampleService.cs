using Aeroverra.StreamDeck.Client;
using Aeroverra.StreamDeck.Client.Events;


namespace StreamDeckClientDevelopment
{
    public class ExampleService : BackgroundService
    {
        private readonly ILogger<ExampleService> _logger;
        private readonly EventManager _eventsManager;
        private readonly IElgatoDispatcher _elgatoDispatcher;
        public ExampleService(ILogger<ExampleService> logger, EventManager eventsManager, IElgatoDispatcher dispatcher)
        {
            _logger = logger;
            _eventsManager = eventsManager;
            _elgatoDispatcher = dispatcher;

            eventsManager.OnKeyUp += HandleKeyUp;
            eventsManager.OnDidReceiveSettings += DidReceiveSettings;
            eventsManager.OnDidReceiveGlobalSettings += DidReceiveGlobalSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }

        public void HandleKeyUp(object? sender, KeyUpEvent e)
        {
            _logger.LogInformation("ExampleService Received event {event}", e.Event);
        }

        public void DidReceiveSettings(object? sender, DidReceiveSettingsEvent e)
        {
            _logger.LogInformation("ExampleService Received event {event}", e.Event);
            _elgatoDispatcher.ShowOk(e.Context);
        }

        public void DidReceiveGlobalSettings(object? sender, DidReceiveGlobalSettingsEvent e)
        {
            _logger.LogInformation("ExampleService Received event {event}", e.Event);
        }

    }
}