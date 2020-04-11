using Deconz2Mqtt.Domain.Model;

namespace Deconz2Mqtt.Domain.MqttMessageHandlers
{
    public class PresenceStateHandler : BooleanSensorToMqttMessageHandler
    {
        protected override bool? Value(State state) => state.Presence;

        protected override string TopicType => "presence";
    }
}