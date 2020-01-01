using Deconz2Mqtt.Domain.Model;

namespace Deconz2Mqtt.Domain.MqttMessageHandlers
{
    public class PressureStateHandler : DecimalSensorToMqttMessageHandler
    {
        protected override decimal? Value(State state) => state.Pressure;
        protected override string TopicType => "pressure";
    }
}