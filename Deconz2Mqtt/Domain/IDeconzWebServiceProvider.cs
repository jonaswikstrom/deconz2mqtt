using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Deconz2Mqtt.Domain
{
    public interface IWebServiceProvider
    {
        Task<JObject> GetState(string uri);
        Task SetState(string uri, string payload);
    }
}