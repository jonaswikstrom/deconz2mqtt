using System;
using System.Timers;
using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deconz2Mqtt.Domain
{
    public class DeconzHeartBeatTimer : IDeconzHeartBeatTimer, IDisposable
    {
        private readonly ILogger<DeconzHeartBeatTimer> logger;
        private readonly Timer timer;
        private Action action;

        public DeconzHeartBeatTimer(ILogger<DeconzHeartBeatTimer> logger, IOptions<DeconzSettings> settings)
        {
            this.logger = logger;
            logger.LogInformation($"Init heart beat update for {settings.Value.HartbeatTimeSpan.TotalMilliseconds} milliseconds");
            timer = new Timer(settings.Value.HartbeatTimeSpan.TotalMilliseconds);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            logger.LogInformation("Heart beat update");
            action?.Invoke();
        }

        public void Start()
        {
            logger.LogInformation($"Heart beat update started");
            timer.Enabled = true;
        }

        public void Stop()
        {
            timer.Enabled = false;
        }

        public void OnHeartBeat(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
