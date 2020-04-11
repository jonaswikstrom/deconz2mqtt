using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain;
using Deconz2Mqtt.Domain.Model;
using Deconz2Mqtt.Domain.MqttMessageHandlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
// ReSharper disable PossibleMultipleEnumeration

namespace Deconz2Mqtt
{

    public class Deconz2MqttHost : IHostedService
    {
        private readonly ILogger<Deconz2MqttHost> logger;
        private readonly IDeconzWebSocketServiceProvider deconzWebSocketServiceProvider;
        private readonly IDeconzWebServiceProvider deconzWebServiceProvider;
        private readonly IDeconzHeartBeatTimer deconzHeartBeatTimer;
        private readonly IMqttClient mqttClient;
        private readonly IHostApplicationLifetime applicationLifetime;
        private readonly IEnumerable<ISensorToMqttMessageHandler> sensorToMqttMessageHandlers;
        private readonly ConcurrentDictionary<string, Sensor> sensorsDictionary;


        public Deconz2MqttHost(ILogger<Deconz2MqttHost> logger, 
            IDeconzWebSocketServiceProvider deconzWebSocketServiceProvider, 
            IDeconzWebServiceProvider deconzWebServiceProvider, 
            IDeconzHeartBeatTimer deconzHeartBeatTimer, 
            IMqttClient mqttClient,
            IHostApplicationLifetime applicationLifetime,
            IEnumerable<ISensorToMqttMessageHandler> sensorToMqttMessageHandlers)
        {
            this.logger = logger;
            this.deconzWebSocketServiceProvider = deconzWebSocketServiceProvider;
            this.deconzWebServiceProvider = deconzWebServiceProvider;
            this.deconzHeartBeatTimer = deconzHeartBeatTimer;
            this.mqttClient = mqttClient;
            this.applicationLifetime = applicationLifetime;
            this.sensorToMqttMessageHandlers = sensorToMqttMessageHandlers.ToArray();

            sensorsDictionary = new ConcurrentDictionary<string, Sensor>();

            deconzHeartBeatTimer.OnHeartBeat(async () =>
            {
                var fullState = await deconzWebServiceProvider.GetFullState();
                CreateOrUpdateSensorsDictionary(fullState);
                await SendSensorsDictionaryData();
            });


            deconzWebSocketServiceProvider.OnMessageReceived(async message =>
            {
                if (!sensorsDictionary.TryGetValue(message.Id, out var sensor)) return;

                var mqttMessages = sensorToMqttMessageHandlers.Select(p => p.HandleState(sensor.Name, message.State))
                    .Where(p => p != null);

                foreach (var mmqttMessage in mqttMessages)
                {
                    await mqttClient.PublishAsync(mmqttMessage);
                }
            });

        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            applicationLifetime.ApplicationStarted.Register(async () => await OnStarted());
            return Task.CompletedTask;
        }

        private async Task OnStarted()
        {
            await mqttClient.ConnectAsync();

            var fullState = await deconzWebServiceProvider.GetFullState();
            CreateOrUpdateSensorsDictionary(fullState);
            await SendSensorsDictionaryData();
            deconzHeartBeatTimer.Start();
            await deconzWebSocketServiceProvider.ConnectAsync();
        }

        public void CreateOrUpdateSensorsDictionary(FullState fullState)
        {
            foreach (var (key, sensor) in fullState.Sensors)
            {
                sensorsDictionary.AddOrUpdate(key, sensor, (_, oldValue) => sensor);
            }
        }

        private async Task SendSensorsDictionaryData()
        {
            foreach (var sensor in sensorsDictionary.Values)
            {
                var messageHandlers =  sensorToMqttMessageHandlers.Select(p => p.HandleState(sensor.Name, sensor.State))
                    .Where(p => p != null);

                foreach (var message in messageHandlers)
                {
                    await mqttClient.PublishAsync(message);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            deconzHeartBeatTimer.Stop();
            await deconzWebSocketServiceProvider.DisconnectAsync();
            await mqttClient.DisconnectAsync();
        }
    }
}