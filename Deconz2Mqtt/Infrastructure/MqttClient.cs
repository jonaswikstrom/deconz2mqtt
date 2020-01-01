using System.Threading;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain;
using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;

namespace Deconz2Mqtt.Infrastructure
{
    public class MqttClient : IMqttClient
    {
        private readonly ILogger<MqttClient> logger;
        private readonly IHostApplicationLifetime applicationLifetime;
        private readonly IOptions<MqttSettings> settings;
        private readonly MQTTnet.Client.IMqttClient client;
        private readonly IMqttClientOptions options;
        private bool disconnecting;

        public MqttClient(ILogger<MqttClient> logger, IHostApplicationLifetime applicationLifetime, IOptions<MqttSettings> settings)
        {
            this.logger = logger;
            this.applicationLifetime = applicationLifetime;
            this.settings = settings;

            var factory = new MqttFactory();
            client = factory.CreateMqttClient();

            options = new MqttClientOptionsBuilder()
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
                .WithTcpServer(settings.Value.HostName)
                .WithCredentials(settings.Value.Username, settings.Value.Password)
                .Build();
        }

        public async Task ConnectAsync()
        {
            logger.LogInformation($"Connecting MQTT on {settings.Value.HostName}");
            await client.ConnectAsync(options, CancellationToken.None);

            client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
            logger.LogInformation("Connected");
        }

        private void OnDisconnected(MqttClientDisconnectedEventArgs mqttClientDisconnectedEventArgs)
        {
            if (disconnecting) return;
            logger.LogError(mqttClientDisconnectedEventArgs.Exception, "MQTT Client disconnected");
            applicationLifetime.StopApplication();
        }

        public void Dispose()
        {
            client?.Dispose();
        }

        public async Task PublishAsync(MqttMessage mqttMessage)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic($"{settings.Value.TopicRoot}/{mqttMessage.Topic}")
                .WithPayload(mqttMessage.Payload)
                .WithRetainFlag()
                .Build();

            await client.PublishAsync(message, CancellationToken.None);
            logger.LogInformation($"Pubished '{settings.Value.TopicRoot}/{mqttMessage.Topic} {mqttMessage.Payload}'");
        }

        public async Task DisconnectAsync()
        {
            if (client == null) return;
            disconnecting = true;
            var disconnectOptions = new MqttClientDisconnectOptions()
            {
               ReasonCode = MqttClientDisconnectReason.NormalDisconnection,
               ReasonString = "Client shutting down"
            };
            await client.DisconnectAsync(disconnectOptions, CancellationToken.None);
        }
    }
}
