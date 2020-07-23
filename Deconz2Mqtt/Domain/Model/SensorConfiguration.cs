using System;

namespace Deconz2Mqtt.Domain.Model
{
    public abstract class EntityConfiguration
    {
        public string Id { get; set; }
        public string StatePath { get; set; }
        public string StateTopic { get; set; }
        public TimeSpan? StateUpdateInterval { get; set; }
    }

    public class MappingsConfiguration
    {
        public SensorConfiguration[] Sensors  { get; set; }
        public LightsConfiguration[] Lights  { get; set; }
    }

    public class SensorConfiguration : EntityConfiguration
    {

    }

    public class LightsConfiguration : EntityConfiguration
    {
        public string CommandTopic { get; set; }
    }
}