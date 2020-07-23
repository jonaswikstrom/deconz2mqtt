using System.Threading.Tasks;
using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Deconz2Mqtt.Domain.Entities
{
    public class Light : Entity
    {
        private readonly ILogger<Light> logger;
        private readonly ITimer timer;
        private readonly IWebServiceProvider webServiceProvider;
        private readonly IWebSocketServiceProvider webSocketServiceProvider;
        private readonly LightsConfiguration lightsConfiguration;

        public Light(ILogger<Light> logger, 
            ITimer timer, 
            IWebServiceProvider webServiceProvider,
            IWebSocketServiceProvider webSocketServiceProvider, 
            IMqttClient mqttClient, 
            LightsConfiguration lightsConfiguration) : 
            base(logger, timer, webSocketServiceProvider, webServiceProvider, mqttClient, lightsConfiguration)
        {
            this.logger = logger;
            this.timer = timer;
            this.webServiceProvider = webServiceProvider;
            this.webSocketServiceProvider = webSocketServiceProvider;
            this.lightsConfiguration = lightsConfiguration;

            mqttClient.OnMessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, MqttMessage e)
        {
            if (!e.Topic.Equals(lightsConfiguration.CommandTopic, System.StringComparison.InvariantCultureIgnoreCase)) return;

            webServiceProvider.SetState($"lights/{lightsConfiguration.Id}/state", e.Payload);
        }

        protected override string EntityType => "lights";
    }
}