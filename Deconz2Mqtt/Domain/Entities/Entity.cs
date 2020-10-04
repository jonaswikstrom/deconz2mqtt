using System;
using System.Globalization;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private string oldPayload;

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

            if(!entityConfiguration.PollOnly) webSocketServiceProvider.OnMessageReceived += OnMessageReceived;

            timer.OnTimerElapsed(async () =>
            {
                await StateUpdate();
            });
        }

        private void OnMessageReceived(object sender, JObject jObject)
        {
            if (!ConcernsThis(jObject)) return;

            var jToken = QueryToken(jObject);
            if (jToken == null)
            {
                logger.LogInformation($"State path '{entityConfiguration.StatePath}' for {GetType().Name.ToLowerInvariant()} id '{entityConfiguration.Id}' returned no value");
                return;
            }

            var payload = jToken.ToString(Formatting.None);
            if (!string.IsNullOrEmpty(oldPayload) && oldPayload.Equals(payload)) return;
            oldPayload = payload;
            PublishPayload(payload).Wait();
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

        protected virtual JToken QueryToken(JObject jObject)
        {
            return jObject.SelectToken(entityConfiguration.StatePath);
        }

        protected virtual async Task PublishPayload(string payload)
        {
            if (entityConfiguration.Decimals.HasValue || entityConfiguration.Divisor.HasValue)
            {
                payload = ParseDecimalTypedPayload(payload);
            }

            var mqttMessage = new MqttMessage(entityConfiguration.StateTopic, payload);
            await mqttClient.PublishAsync(mqttMessage);
        }


        protected virtual string ParseDecimalTypedPayload(string payload)
        {
            if (!decimal.TryParse(payload, out var decimalPayload))
            {
                logger.LogWarning(
                    $"Payload '{payload}' for {EntityTypeName} with id '{entityConfiguration.Id}' is defined to be a decimal, but is not");

                return payload;
            }

            decimalPayload = decimalPayload / (entityConfiguration.Divisor ?? 1);
            decimalPayload = Math.Round(decimalPayload, entityConfiguration.Decimals ?? 2);

            return decimalPayload.ToString(CultureInfo.InvariantCulture);
        }

        protected string EntityTypeName => GetType().Name.ToLowerInvariant();

        public virtual Task Stop()
        {
            logger.LogInformation($"Stopping {EntityTypeName} {entityConfiguration.Id}");
            timer?.Stop();
            webSocketServiceProvider.OnMessageReceived -= OnMessageReceived;
            return Task.CompletedTask;
        }

        public virtual async Task Start()
        {
            logger.LogInformation($"Starting {EntityTypeName} {entityConfiguration.Id}");

            if (!entityConfiguration.IgnoreStateUpdateAtStartup)
            {
                await StateUpdate();
            }
            if (!entityConfiguration.StateUpdateInterval.HasValue) return;
            timer.Start(entityConfiguration.StateUpdateInterval.Value);
        }

        private async Task StateUpdate()
        {
            logger.LogInformation($"Manual state read for {EntityTypeName} {entityConfiguration.Id}");
            var jObject = await webServiceProvider.GetState($"{EntityType}/{entityConfiguration.Id}");

            if (jObject == null)
            {
                logger.LogWarning($"No state information for {EntityTypeName} with id '{entityConfiguration.Id}'");
                return;
            }

            var token = QueryToken(jObject);
            await PublishPayload(token.ToString());
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}