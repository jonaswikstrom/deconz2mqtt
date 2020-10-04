using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Deconz2Mqtt.Domain.Entities
{
    public class Sensor : Entity
    {
        private readonly ILogger logger;
        private readonly SensorConfiguration sensorConfig;


        public Sensor(ILogger logger,
            ITimer timer,
            IWebServiceProvider webServiceProvider,
            IWebSocketServiceProvider webSocketServiceProvider,
            IMqttClient mqttClient,
            SensorConfiguration sensorConfig) :
            base(logger, timer, webSocketServiceProvider, webServiceProvider, mqttClient, sensorConfig)
        {
            this.logger = logger;
            this.sensorConfig = sensorConfig;
        }

        protected override string EntityType => "sensors";
    }
}