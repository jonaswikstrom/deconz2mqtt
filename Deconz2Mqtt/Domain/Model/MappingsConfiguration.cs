namespace Deconz2Mqtt.Domain.Model
{
    public class MappingsConfiguration
    {
        public SensorConfiguration[] Sensors  { get; set; }
        public LightsConfiguration[] Lights  { get; set; }
    }
}