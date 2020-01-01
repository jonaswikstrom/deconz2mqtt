using Deconz2Mqtt.Domain.Model;

namespace Deconz2Mqtt.Domain.MqttMessageHandlers
{
    public interface ISensorToMqttMessageHandler
    {
        MqttMessage HandleState(string sensorName, State state);
    }
}