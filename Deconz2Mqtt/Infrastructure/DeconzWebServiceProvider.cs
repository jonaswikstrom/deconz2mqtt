using System;
using System.Net.Http;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain;
using Deconz2Mqtt.Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deconz2Mqtt.Infrastructure
{
    public class DeconzWebServiceProvider : IDeconzWebServiceProvider
    {
        private readonly ILogger<DeconzWebServiceProvider> logger;
        private readonly IOptions<DeconzSettings> settings;
        private readonly HttpClient httpClient;

        public DeconzWebServiceProvider(ILogger<DeconzWebServiceProvider> logger, IOptions<DeconzSettings> settings)
        {
            this.logger = logger;
            this.settings = settings;
            httpClient = new HttpClient()
            {
                BaseAddress =  new Uri($"http://{settings.Value.HostName}:{settings.Value.Port}/api/{settings.Value.ApiKey}/")
            };
        }

        public async Task<FullState> GetFullState()
        {
            var response = await httpClient.GetAsync(string.Empty);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<FullState>();
        }
    }
}