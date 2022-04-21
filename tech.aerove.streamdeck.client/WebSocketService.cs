using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Actions;
using tech.aerove.streamdeck.client.Events;
using tech.aerove.streamdeck.client.Pipeline;
using tech.aerove.streamdeck.client.Startup;

namespace tech.aerove.streamdeck.client
{
    public class WebSocketService : BackgroundService
    {
        private ClientWebSocket Socket = new ClientWebSocket();
        private CancellationToken StoppingToken { get; set; }
        private readonly SemaphoreSlim SendLock = new SemaphoreSlim(1);
        private Task Listener { get; set; }
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebSocketService> _logger;
        private readonly StreamDeckInfo StreamDeckInfo;
        private readonly IPipeline _pipeline;
        public WebSocketService(IConfiguration configuration, ILogger<WebSocketService> logger, StreamDeckInfo streamDeckInfo, IPipeline pipeline)
        {
            _configuration = configuration;
            _logger = logger;
            StreamDeckInfo = streamDeckInfo;
            _pipeline = pipeline;
            _pipeline.SetWebSocket(this);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var logParametersOnly = _configuration.GetValue<bool>("DevLogParametersOnly");
            if (logParametersOnly)
            {
                _logger.LogInformation("WebSocketService did not startup. Logging parameters only.");
                return;
            }

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
                            _logger.LogError("Received binary instead of text from Elgato. {Binary}", buffer.Take(response.Count).ToArray());
                            continue;
                        }
                    }
                    //WebSocketMessageType.Text

                    try
                    {
                        await _pipeline.HandleIncoming(result);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error when handling event.{EventData}", result);
                    }



                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Listener died in exception.");
            }

        }

        public async Task SendAsync(object message)
        {

            if(message is JObject)
            {
                //Convert casing because the default  converter does not handle jobjects properly
                //causing the streamdeck to ignore events
                message = JObject.FromObject((message as JObject).ToObject<ExpandoObject>(), JsonSerializer.Create(new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
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
