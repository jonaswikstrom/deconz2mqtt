using System;
using System.IO;
using System.Threading.Tasks;
using Deconz2Mqtt.Domain;
using Deconz2Mqtt.Domain.Model;
using Deconz2Mqtt.Domain.MqttMessageHandlers;
using Deconz2Mqtt.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Deconz2Mqtt
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting host for Deconz2Mqtt");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, true)
                .AddEnvironmentVariables()
                .Build();

            var hostBuilder = new HostBuilder().ConfigureServices(p =>
            {
                p.AddLogging(p => p.AddConsole())
                    .AddSingleton<IDeconzHeartBeatTimer, DeconzHeartBeatTimer>()
                    .AddSingleton<IDeconzWebServiceProvider, DeconzWebServiceProvider>()
                    .AddSingleton<IDeconzWebSocketServiceProvider, DeconzWebSocketServiceProvider>()
                    .AddSingleton<IMqttClient, MqttClient>()
                    .AddSingleton<ISensorToMqttMessageHandler, HumidityStateHandler>()
                    .AddSingleton<ISensorToMqttMessageHandler, TemperatureStateHandler>()
                    .AddSingleton<ISensorToMqttMessageHandler, PressureStateHandler>()
                    .AddSingleton<IConfiguration>(configuration)
                    .Configure<DeconzSettings>(configuration.GetSection("Deconz"))
                    .Configure<MqttSettings>(configuration.GetSection("Mqtt"))
                    .AddHostedService<Deconz2MqttHost>();
            });

            await hostBuilder.RunConsoleAsync();
        }
    }
}
