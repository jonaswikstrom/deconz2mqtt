using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain;
using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Deconz2Mqtt.Infrastructure
{
    public class DeconzWebSocketServiceProvider  : IDeconzWebSocketServiceProvider, IDisposable
    {
        private readonly ILogger<DeconzWebSocketServiceProvider> logger;
        private readonly DeconzSettings settings;
        private readonly ClientWebSocket webSocket;
        private Action<WebSocketMessage> action;

        public DeconzWebSocketServiceProvider(ILogger<DeconzWebSocketServiceProvider> logger, IOptions<DeconzSettings> settings)
        {
            this.logger = logger;
            this.settings = settings.Value;
            webSocket = new ClientWebSocket();
        }

        private Uri Uri => new Uri($"ws://{settings.HostName}:{settings.WebSocketPort}");

        public async Task ConnectAsync()
        {
            logger.LogInformation($"Connecting to {Uri}");
            await webSocket.ConnectAsync(Uri, CancellationToken.None);
            logger.LogInformation("Connected");

            await ReceiveAsync();
        }

        public async Task DisconnectAsync()
        {
            logger.LogInformation("Closing connection");
            await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

            logger.LogInformation("Closed");
        }

        public void OnMessageReceived(Action<WebSocketMessage> action)
        {
            this.action = action;
        }

        private async Task ReceiveAsync()
        {
            logger.LogInformation("Start listening for state chages");

            var buffer = new ArraySegment<byte>(new byte[8192]);
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = null;
                await using var ms = new MemoryStream();
                do
                {
                    result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                    if (webSocket.State != WebSocketState.Open) return;
                    ms.Write(buffer.Array ?? throw new InvalidOperationException(), buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    logger.LogInformation("Message type close received");
                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);

                    if (result.MessageType != WebSocketMessageType.Text) continue;
                    using var reader = new StreamReader(ms, Encoding.UTF8);
                    var jsonMessage = await reader.ReadToEndAsync();
                    logger.LogInformation($"Received message: '{jsonMessage}'");

                    var fullState = JsonConvert.DeserializeObject<WebSocketMessage>(jsonMessage);
                    action?.Invoke(fullState);
                }
            }

            logger.LogInformation("Stopped listening for state chages");
        }

        public void Dispose()
        {
            webSocket?.Dispose();
        }
    }
}