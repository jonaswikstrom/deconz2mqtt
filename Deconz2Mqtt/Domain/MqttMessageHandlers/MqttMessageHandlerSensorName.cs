using System.Text.RegularExpressions;

namespace Deconz2Mqtt.Domain.MqttMessageHandlers
{
    public class MqttMessageHandlerSensorName
    {
        private readonly string name;

        public MqttMessageHandlerSensorName(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return Regex.Replace(name, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToLowerInvariant();
        }

    }
}