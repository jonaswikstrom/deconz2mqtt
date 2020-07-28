namespace Deconz2Mqtt.Domain.Model
{
    public class SensorConfiguration : EntityConfiguration
    {
        public int? Divisor { get; set; }
        public int? Decimals { get; set; }
    }
}