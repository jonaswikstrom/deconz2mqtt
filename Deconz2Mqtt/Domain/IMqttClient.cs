using System;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain.Model;

namespace Deconz2Mqtt.Domain
{
    public interface IMqttClient : IDisposable
    { 
        Task ConnectAsync();
        Task PublishAsync(MqttMessage mqttMessage);
        Task DisconnectAsync();

        Task Subscribe(string topic);
        Task UnSubscribe(string topic);

        event EventHandler<MqttMessage> OnMessageReceived;
    }
}