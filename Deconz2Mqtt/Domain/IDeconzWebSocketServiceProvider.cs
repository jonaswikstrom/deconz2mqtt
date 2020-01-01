using System;
using System.Threading;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain.Model;

namespace Deconz2Mqtt.Domain
{
    public interface IDeconzWebSocketServiceProvider
    {
        Task ConnectAsync();
        Task DisconnectAsync();

        void OnMessageReceived(Action<WebSocketMessage> action);
    }
}