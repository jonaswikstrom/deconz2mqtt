using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain;
using Deconz2Mqtt.Domain.Entities;
using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ReSharper disable PossibleMultipleEnumeration

namespace Deconz2Mqtt
{

    public class Deconz2MqttHost : IHostedService
    {
        private readonly ILogger<Deconz2MqttHost> logger;
        private readonly IWebServiceProvider webServiceProvider;
        private readonly IWebSocketServiceProvider webSocketServiceProvider;
        private readonly IMqttClient mqttClient;
        private readonly IHostApplicationLifetime applicationLifetime;
        private readonly IOptions<MappingsConfiguration> mappingConfiguration;
        private Sensor[] sensors;
        private Light[] lights;

        public Deconz2MqttHost(ILogger<Deconz2MqttHost> logger,
            IWebServiceProvider webServiceProvider,
            IWebSocketServiceProvider webSocketServiceProvider,
            IMqttClient mqttClient,
            IHostApplicationLifetime applicationLifetime,
            IOptions<MappingsConfiguration> mappingConfiguration)
        {
            this.logger = logger;
            this.webServiceProvider = webServiceProvider;
            this.webSocketServiceProvider = webSocketServiceProvider;
            this.mqttClient = mqttClient;
            this.applicationLifetime = applicationLifetime;
            this.mappingConfiguration = mappingConfiguration;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            applicationLifetime.ApplicationStarted.Register(async () => await OnStarted());
            return Task.CompletedTask;
        }

        private async Task OnStarted()
        {
            var tasks = new[]
            {
                mqttClient.ConnectAsync(),
                webSocketServiceProvider.ConnectAsync(),
                InitiateSensors(),
                InitiateLights()
            };

            await Task.WhenAll(tasks);
        }

        private async Task InitiateSensors()
        {
            logger.LogInformation("Initiating sensors");

            sensors = mappingConfiguration.Value.Sensors.Select(s => 
                new Sensor(logger, new Domain.Timer(logger), webServiceProvider, webSocketServiceProvider, mqttClient, s))
                .ToArray();

            foreach (var sensor in sensors)
            {
                await sensor.Start();
            }
        }

        private async Task InitiateLights()
        {
            logger.LogInformation("Initiating lights");

            lights = mappingConfiguration.Value.Lights.Select(s =>
                    new Light(logger, new Domain.Timer(logger), webServiceProvider, webSocketServiceProvider, mqttClient, s))
                .ToArray();

            foreach (var light in lights)
            {
                await light.Start();
            }
        }


        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await webSocketServiceProvider.DisconnectAsync();
            await mqttClient.DisconnectAsync();

            foreach (var sensor in sensors)
            {
                await sensor.Stop();
            }

            foreach (var light in lights)
            {
                await light.Stop();
            }
        }
    }
}