using System;

namespace Deconz2Mqtt.Domain
{
    public interface IDeconzHeartBeatTimer
    {
        void Start();
        void Stop();
        void OnHeartBeat(Action action);
    }
}