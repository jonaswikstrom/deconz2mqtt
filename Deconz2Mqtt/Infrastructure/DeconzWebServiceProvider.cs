using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain;
using Deconz2Mqtt.Domain.Entities;
using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        public async Task SetState(string uri, string payload)
        {
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            await httpClient.PutAsync(uri, content);
        }
    }
}