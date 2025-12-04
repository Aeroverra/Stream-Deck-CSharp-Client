using Aeroverra.StreamDeck.Client.Startup;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Dynamic;
using System.Net.WebSockets;
using System.Text;

namespace Aeroverra.StreamDeck.Client
{
    internal class ElgatoWebSocket(ILogger<ElgatoWebSocket> logger, StreamDeckInfo streamDeckInfo) : IElgatoWebSocket
    {
        private ClientWebSocket _socket = new ClientWebSocket();

        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1);

        private CancellationTokenSource? _cancellationTokenSource;

        private readonly JsonSerializer _jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        public async Task ConnectAsync()
        {
            if (_socket.State == WebSocketState.Open)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            var uri = new Uri($"ws://localhost:{streamDeckInfo.Port}");
            await _socket.ConnectAsync(uri, _cancellationTokenSource.Token);

            if (_socket.State != WebSocketState.Open)
            {
                logger.LogCritical("Could not connect to Elgato {uri}", uri);
                await DisconnectAsync();
                return;
            }

            var registrationMessage = new
            {
                UUID = streamDeckInfo.PluginUUID,
                Event = streamDeckInfo.RegisterEvent
            };

            await SendAsync(registrationMessage);
            logger.LogDebug("Listening on {uri}", uri);

        }

        public async Task SendAsync(object message)
        {
            if (_cancellationTokenSource == null)
                return;

            if (message is JObject)
            {
                //Convert casing because the default  converter does not handle jobjects properly
                //causing the streamdeck to ignore events
                message = JObject.FromObject((message as JObject)!.ToObject<ExpandoObject>()!, _jsonSerializer);
            }

            var json = JsonConvert.SerializeObject(message, _jsonSerializerSettings);

            try
            {
                await _sendLock.WaitAsync();
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                await _socket.SendAsync(buffer, WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Could not send message to Elgato: {message}", json);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        public async Task<string?> ListenAsync()
        {
            var buffer = new ArraySegment<byte>(new byte[256]);
            var readBytes = new List<byte>();
            string? result = null;
            try
            {

                using (var memoryStream = new MemoryStream())
                {
                    WebSocketReceiveResult response;
                    do
                    {
                        response = await _socket.ReceiveAsync(buffer, _cancellationTokenSource.Token);
                        if (buffer.Array != null)
                        {
                            memoryStream.Write(buffer.Array, buffer.Offset, response.Count);
                        }
                    }
                    while (!response.EndOfMessage && _cancellationTokenSource?.IsCancellationRequested == false);
                    using (StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8))
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        result = reader.ReadToEnd();
                    }
                    if (response.MessageType == WebSocketMessageType.Close)
                    {
                        return null;
                    }
                    if (response.MessageType == WebSocketMessageType.Binary)
                    {
                        logger.LogError("Received binary instead of text from Elgato. {Binary}", buffer.Take(response.Count).ToArray());
                        return null;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Listener died in exception.");
            }
            return null;
        }

        public async Task DisconnectAsync()
        {
            if (_cancellationTokenSource == null)
                return;

            _cancellationTokenSource.CancelAfter(5000);

            try
            {
                await _socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Aerove Stream Deck Client Disconnecting",
                    _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                logger.LogError("Could not kill web socket connection gracefully.");
            }
            catch (Exception e)
            {
                logger.LogInformation(e, "Error killing socket connection.");
            }
            finally
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            try
            {
                _socket.Dispose();
            }
            catch { }
            _socket = new ClientWebSocket();
        }
    }
}
