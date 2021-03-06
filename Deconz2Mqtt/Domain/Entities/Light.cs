﻿using System.Linq;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Deconz2Mqtt.Domain.Entities
{
    public class Light : Entity
    {
        private readonly IWebServiceProvider webServiceProvider;
        private readonly IMqttClient mqttClient;
        private readonly LightsConfiguration lightsConfiguration;

        public Light(ILogger logger, 
            ITimer timer, 
            IWebServiceProvider webServiceProvider,
            IWebSocketServiceProvider webSocketServiceProvider, 
            IMqttClient mqttClient, 
            LightsConfiguration lightsConfiguration) : 
            base(logger, timer, webSocketServiceProvider, webServiceProvider, mqttClient, lightsConfiguration)
        {
            this.webServiceProvider = webServiceProvider;
            this.mqttClient = mqttClient;
            this.lightsConfiguration = lightsConfiguration;

            mqttClient.OnMessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, MqttMessage e)
        {
            if (!e.Topic.Equals($"{mqttClient.TopicRoot}/{lightsConfiguration.CommandTopic}", System.StringComparison.InvariantCultureIgnoreCase)) return;

            var lastPath = lightsConfiguration.StatePath.Split('.').Last();
            var payload = string.Concat("{\"", lastPath, "\":" , e.Payload, "}");
            webServiceProvider.SetState($"lights/{lightsConfiguration.Id}/state", payload);
        }

        protected override string EntityType => "lights";

        public override async Task Stop()
        {
            await base.Stop();
            await mqttClient.UnSubscribe(lightsConfiguration.CommandTopic);
            mqttClient.OnMessageReceived -= OnMessageReceived;
        }

        public override async Task Start()
        {
            await base.Start();
            await mqttClient.Subscribe( lightsConfiguration.CommandTopic);
        }
    }
}