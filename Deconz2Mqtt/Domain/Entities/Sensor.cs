using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Deconz2Mqtt.Domain.Entities
{
    public class Sensor : Entity {


            public Sensor(ILogger logger,
                ITimer timer,
                IWebServiceProvider webServiceProvider,
                IWebSocketServiceProvider webSocketServiceProvider,
                IMqttClient mqttClient,
                EntityConfiguration sensorConfig) : 
                base(logger, timer, webSocketServiceProvider, webServiceProvider, mqttClient, sensorConfig)
            {
            }

            protected override string EntityType => "sensors";
    }
}