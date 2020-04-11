using System.Globalization;
using Deconz2Mqtt.Domain.Model;

namespace Deconz2Mqtt.Domain.MqttMessageHandlers
{
    public abstract class BooleanSensorToMqttMessageHandler : ISensorToMqttMessageHandler
    {
        protected abstract bool? Value(State state);
        protected abstract string TopicType { get; }

        public MqttMessage HandleState(string sensorName, State state)
        {
            var value = Value(state);
            return !value.HasValue ? null : new MqttMessage($"sensor/{new MqttMessageHandlerSensorName(sensorName)}/{TopicType.ToLower()}", value.Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}