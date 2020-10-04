using System;
using System.Threading.Tasks;
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