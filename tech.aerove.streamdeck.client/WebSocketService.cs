using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Actions;
using tech.aerove.streamdeck.client.Events;

namespace tech.aerove.streamdeck.client
{
    public class WebSocketService : BackgroundService
    {
        //Todo: Test ClientWebSocket For Mac
        //It's unclear if this works on mac. 
        //If not I need to make my own implementation
        private ClientWebSocket Socket = new ClientWebSocket();
        private CancellationToken StoppingToken { get; set; }
        private readonly SemaphoreSlim SendLock = new SemaphoreSlim(1);
        private Task Listener { get; set; }
        private readonly ILogger<WebSocketService> _logger;
        private readonly StreamDeckInfo StreamDeckInfo;
        private readonly ElgatoEventHandler _eventHandler;
        public WebSocketService(ILogger<WebSocketService> logger, StreamDeckInfo streamDeckInfo, ElgatoEventHandler eventHandler)
        {
            _logger = logger;
            StreamDeckInfo = streamDeckInfo;
            _eventHandler = eventHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var devenv = Environment.ProcessPath == "C:\\Users\\Nicholas\\Desktop\\Repos\\tech.aerove.streamdeck.service\\tech.aerove.streamdeck.service\\bin\\Debug\\net6.0\\tech.aerove.streamdeck.service.exe";
            if (!devenv) { return; }
            StoppingToken = stoppingToken;
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await ConnectAsync(stoppingToken);
                    await Task.Delay(60000, stoppingToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "websocketservice error");
            }
            await DisconnectAsync();
            _logger.LogInformation("{ClientName} Shutting Down...", "Aerove Stream Deck Client");
        }




        private async Task ConnectAsync(CancellationToken stoppingToken)
        {
            if (Socket.State == WebSocketState.Open)
            {
                return;
            }
            var uri = new Uri($"ws://localhost:{StreamDeckInfo.Port}");
            await Socket.ConnectAsync(uri, stoppingToken);
            if (Socket.State != WebSocketState.Open)
            {
                _logger.LogWarning("Could not connect to Elgato {uri}", uri);
                await DisconnectAsync();
                return;
            }
            var registrationMessage = new
            {
                UUID = StreamDeckInfo.PluginUUID,
                Event = StreamDeckInfo.RegisterEvent
            };
            await SendAsync(registrationMessage);
            Listener = ListenAsync();
            _logger.LogDebug("Listening on {uri}", uri);

        }

        private async Task ListenAsync()
        {
            var buffer = new ArraySegment<byte>(new byte[256]);
            var readBytes = new List<byte>();
            string result = "";
            try
            {
                while (!StoppingToken.IsCancellationRequested)
                {

                    using (var ms = new MemoryStream())
                    {
                        WebSocketReceiveResult response;
                        do
                        {
                            response = await Socket.ReceiveAsync(buffer, StoppingToken);
                            if (buffer.Array != null)
                            {
                                ms.Write(buffer.Array, buffer.Offset, response.Count);
                            }
                        }
                        while (!response.EndOfMessage);
                        using (StreamReader reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            ms.Seek(0, SeekOrigin.Begin);
                            result = reader.ReadToEnd();
                        }
                        if (response.MessageType == WebSocketMessageType.Close)
                        {
                            return;
                        }
                        if (response.MessageType == WebSocketMessageType.Binary)
                        {
                            _logger.LogError("Recieved binary instead of text from Elgato. {Binary}", buffer.Take(response.Count).ToArray());
                            continue;
                        }
                    }
                    //WebSocketMessageType.Text
                    await _eventHandler.HandleIncoming(result);

                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Listener died in exception.");
            }

        }
       
  
       
        public async Task SendAsync(object message)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            var json = JsonConvert.SerializeObject(message, settings);
            try
            {
                await SendLock.WaitAsync();
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                await Socket.SendAsync(buffer, WebSocketMessageType.Text, true, StoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Could not send message to Elgato: {message}", json);
            }
            finally
            {
                SendLock.Release();
            }
        }

        private async Task DisconnectAsync()
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(5000);
            try
            {
                await Socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Aerove Stream Deck Client Disconnecting",
                    tokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("Could not kill web socket connection gracefully.");
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "Error killing socket connection.");
            }
            try
            {
                Socket.Dispose();
            }
            catch { }
            Socket = new ClientWebSocket();
        }
    }
}
