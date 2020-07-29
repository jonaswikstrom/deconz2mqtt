using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain;
using Deconz2Mqtt.Domain.Entities;
using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Deconz2Mqtt.Infrastructure
{
    public class WebServiceProvider : IWebServiceProvider
    {
        private readonly ILogger<WebServiceProvider> logger;
        private readonly IOptions<DeconzSettings> settings;
        private readonly HttpClient httpClient;

        public WebServiceProvider(ILogger<WebServiceProvider> logger, IOptions<DeconzSettings> settings)
        {
            this.logger = logger;
            this.settings = settings;
            httpClient = new HttpClient()
            {
                BaseAddress =  new Uri($"http://{settings.Value.HostName}:{settings.Value.Port}/api/{settings.Value.ApiKey}/")
            };
        }


        public async Task<JObject> GetState(string uri)
        {
            var response = await httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode) return null;
            var jObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            return jObject;
        }

        public async Task<bool> SetState(string uri, string payload)
        {
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var result = await httpClient.PutAsync(uri, content);

            if (result.IsSuccessStatusCode) return true;

            var error = await result.Content.ReadAsAsync<ErrorInformation>();
            logger.LogWarning($"Could not set state on '{error.Error.Address}', '{error.Error.Description}'");
            return false;
        }

        public class ErrorInformation
        {
            [JsonProperty("error")]
            public Error Error { get; set; }
        }

        public class Error
        {
            [JsonProperty("address")]
            public string Address { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("type")]
            public long Type { get; set; }
        }
    }
}