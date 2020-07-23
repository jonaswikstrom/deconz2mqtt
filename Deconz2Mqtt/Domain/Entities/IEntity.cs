using System;
using System.Threading.Tasks;

namespace Deconz2Mqtt.Domain.Entities
{
    public interface IEntity : IDisposable
    {
        Task Start();
        Task Stop();
    }
}