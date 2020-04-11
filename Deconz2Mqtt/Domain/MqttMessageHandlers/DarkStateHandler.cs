using Deconz2Mqtt.Domain.Model;

namespace Deconz2Mqtt.Domain.MqttMessageHandlers
{
    public class DarkStateHandler : BooleanSensorToMqttMessageHandler
    {
        protected override bool? Value(State state) => state.Dark;

        protected override string TopicType => "dark";
    }
}