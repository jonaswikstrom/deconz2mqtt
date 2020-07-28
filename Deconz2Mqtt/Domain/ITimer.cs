using System;

namespace Deconz2Mqtt.Domain
{
    public interface ITimer : IDisposable
    {
        void Start(TimeSpan timeSpan);
        void Stop();
        void OnTimerElapsed(Action action);
    }
}