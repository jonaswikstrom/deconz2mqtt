using System.Globalization;
using Deconz2Mqtt.Domain.Model;

namespace Deconz2Mqtt.Domain.MqttMessageHandlers
{
    public abstract class DecimalSensorToMqttMessageHandler : ISensorToMqttMessageHandler
    {
        protected abstract decimal? Value(State state);
        protected abstract string TopicType { get; }
        protected virtual decimal Fraction => 1;

        public MqttMessage HandleState(string sensorName, State state)
        {
            var value = Value(state);
            if (!value.HasValue) return null;

            value = value.Value / Fraction;

            return new MqttMessage($"sensor/{new MqttMessageHandlerSensorName(sensorName)}/{TopicType.ToLower()}", value.Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}