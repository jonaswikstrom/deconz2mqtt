using Deconz2Mqtt.Domain.Model;

namespace Deconz2Mqtt.Domain.MqttMessageHandlers
{
    public class HumidityStateHandler : DecimalSensorToMqttMessageHandler
    {
        protected override decimal Fraction => 100;
        protected override decimal? Value(State state) => state.Humidity;
        protected override string TopicType => "humidity";
    }
}
