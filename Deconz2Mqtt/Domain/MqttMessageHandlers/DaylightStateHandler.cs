using Deconz2Mqtt.Domain.Model;

namespace Deconz2Mqtt.Domain.MqttMessageHandlers
{
    public class DaylightStateHandler : BooleanSensorToMqttMessageHandler
    {
        protected override bool? Value(State state) => state.Daylight;

        protected override string TopicType => "daylight";
    }
}