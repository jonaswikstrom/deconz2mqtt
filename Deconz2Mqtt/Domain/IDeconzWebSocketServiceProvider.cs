using System;
using System.Threading;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain.Model;
using Newtonsoft.Json.Linq;

namespace Deconz2Mqtt.Domain
{
    public interface IWebSocketServiceProvider
    {
        Task ConnectAsync();
        Task DisconnectAsync();

        event EventHandler<JObject> OnMessageReceived;
    }
}