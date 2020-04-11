using Deconz2Mqtt.Domain.Model;

namespace Deconz2Mqtt.Domain.MqttMessageHandlers
{
    public class LuxStateHandler : IntegerSensorToMqttMessageHandler
    {
        protected override int? Value(State state) => state.Lux;

        protected override string TopicType => "lux";
    }
}