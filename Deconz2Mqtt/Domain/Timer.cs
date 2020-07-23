using System;
using System.Timers;
using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deconz2Mqtt.Domain
{
    public class Timer : ITimer
    {
        private readonly ILogger logger;
        private readonly System.Timers.Timer timer;
        private Action action;

        public Timer(ILogger logger)
        {
            this.logger = logger;

            timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            logger.LogInformation("State update");
            action?.Invoke();
        }

        public void Start(TimeSpan timeSpan)
        {
            logger.LogInformation("Manual state update timer started");

            timer.Interval = timeSpan.TotalMilliseconds;
            timer.Enabled = true;
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void OnTimerElapsed(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
