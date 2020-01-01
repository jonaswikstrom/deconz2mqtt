using Deconz2Mqtt.Domain.Model;

namespace Deconz2Mqtt.Domain.MqttMessageHandlers
{
    public class TemperatureStateHandler : DecimalSensorToMqttMessageHandler
    {
        protected override decimal Fraction => 100;
        protected override decimal? Value(State state) => state.Temperature;
        protected override string TopicType => "temperature";
    }
}