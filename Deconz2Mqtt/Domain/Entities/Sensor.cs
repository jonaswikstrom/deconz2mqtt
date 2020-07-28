using System;
using System.Globalization;
using System.Threading.Tasks;
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

        protected override Task PublishPayload(string payload)
        {
            if (!sensorConfig.Divisor.HasValue && !sensorConfig.Decimals.HasValue) return base.PublishPayload(payload);

            if (!decimal.TryParse(payload, out var decimalPayload))
            {
                logger.LogWarning(
                    $"Payload '{payload}' for sensor id '{sensorConfig.Id}' is defined to be a decimal, but is not");

                return base.PublishPayload(payload);
            }

            var divisor = sensorConfig.Divisor ?? 1;
            decimalPayload = decimalPayload / divisor;

            if (sensorConfig.Decimals.HasValue)
            {
                decimalPayload = Math.Round(decimalPayload, sensorConfig.Decimals.Value);
            }

            return base.PublishPayload(decimalPayload.ToString(CultureInfo.InvariantCulture));
        }
    }
}