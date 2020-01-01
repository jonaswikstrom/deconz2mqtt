using System.Threading.Tasks;
using Deconz2Mqtt.Domain.Model;

namespace Deconz2Mqtt.Domain
{
    public interface IDeconzWebServiceProvider
    {
        Task<FullState> GetFullState();
    }
}