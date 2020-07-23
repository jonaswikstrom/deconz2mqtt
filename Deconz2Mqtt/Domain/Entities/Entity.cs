using System;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Deconz2Mqtt.Domain.Entities
{
    public abstract class Entity : IEntity
    {
        private readonly ILogger logger;
        private readonly IWebServiceProvider webServiceProvider;
        private readonly IMqttClient mqttClient;
        private readonly EntityConfiguration entityConfiguration;
        private readonly ITimer timer;
        private readonly IWebSocketServiceProvider webSocketServiceProvider;

        protected Entity(ILogger logger,
            ITimer timer,
            IWebSocketServiceProvider webSocketServiceProvider,
            IWebServiceProvider webServiceProvider,
            IMqttClient mqttClient, 
            EntityConfiguration entityConfiguration)
        {
            this.logger = logger;
            this.timer = timer;
            this.webSocketServiceProvider = webSocketServiceProvider;
            this.webServiceProvider = webServiceProvider;
            this.mqttClient = mqttClient;
            this.entityConfiguration = entityConfiguration;

            webSocketServiceProvider.OnMessageReceived += OnMessageReceived;

            timer.OnTimerElapsed(async () =>
            {
                await StateUpdate();
            });
        }

        private void OnMessageReceived(object sender, JObject jObject)
        {
            if (!ConcernsThis(jObject)) return;
            QueryAndPublishToken(jObject).Wait();
        }

        protected abstract string EntityType { get; }

        protected virtual bool ConcernsThis(JObject jObject)
        {
            return jObject.TryGetValue("id", out var valueToken) &&
                   jObject.TryGetValue("r", out var entityTypeToken) &&
                   valueToken != null &&
                   entityTypeToken != null &&
                   valueToken.ToString().Equals(entityConfiguration.Id, System.StringComparison.InvariantCultureIgnoreCase) &&
                   entityTypeToken.ToString().Equals(EntityType, System.StringComparison.InvariantCultureIgnoreCase);
        }

        protected virtual async Task QueryAndPublishToken(JObject jObject)
        {
            var query = jObject.SelectToken(entityConfiguration.StatePath);

            if (string.IsNullOrEmpty(query.ToString()))
            {
                logger.LogWarning($"State path '{entityConfiguration.StatePath}' for {GetType().Name.ToLowerInvariant()} id '{entityConfiguration.Id}' returns no value");
                return;
            }

            var mqttMessage = new MqttMessage(entityConfiguration.StateTopic, query.ToString());
            await mqttClient.PublishAsync(mqttMessage);
        }

        public virtual Task Stop()
        {
            logger.LogInformation($"Stopping {GetType().Name.ToLowerInvariant()} {entityConfiguration.Id}");
            timer?.Stop();
            webSocketServiceProvider.OnMessageReceived -= OnMessageReceived;
            return Task.CompletedTask;
        }

        public async Task Start()
        {
            logger.LogInformation($"Starting {GetType().Name.ToLowerInvariant()} {entityConfiguration.Id}");

            await StateUpdate();
            if (!entityConfiguration.StateUpdateInterval.HasValue) return;
            timer.Start(entityConfiguration.StateUpdateInterval.Value);
        }

        private async Task StateUpdate()
        {
            logger.LogInformation($"Manual state read for {GetType().Name.ToLowerInvariant()} {entityConfiguration.Id}");
            var jObject = await webServiceProvider.GetState($"{EntityType}/{entityConfiguration.Id}");

            if (jObject == null)
            {
                logger.LogWarning($"No state information for {GetType().Name.ToLowerInvariant()} with id '{entityConfiguration.Id}'");
                return;
            }

            await QueryAndPublishToken(jObject);
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}