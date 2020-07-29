using System;

namespace Deconz2Mqtt.Domain.Model
{
    public abstract class EntityConfiguration
    {
        public string Id { get; set; }
        public string StatePath { get; set; }
        public string StateTopic { get; set; }
        public TimeSpan? StateUpdateInterval { get; set; }

        public string Type { get; set; }
        public int? Divisor { get; set; }
        public int? Decimals { get; set; }
    }
}