using Serilog;
using StreamDeckClientDevelopment;
using Aeroverra.StreamDeck.Client;

internal class Program
{
    //streamdeck://plugins/message/vi.aero.streamdeckclientdevelopment/hello
    //streamdeck://plugins/restart/vi.aero.streamdeckclientdevelopment
    private static async Task Main(string[] args)
    {
        SerilogConfig.Configure();
        IHost host = Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                services.AddAeroveStreamDeckClient(context);
                services.AddHostedService<ExampleService>();
            })
            .Build();

        await host.RunAsync();
    }
}