using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using ServiceBusDuplicateDetection.Consumer.Components;
using ServiceBusDuplicateDetection.Consumer.Consumers;

namespace ServiceBusDuplicateDetection.Consumer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args);
            await host.Build().RunAsync();

        }
        
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, builder) =>
                {
                    var configuration = new ConfigurationBuilder()
                        .Build();
                    var logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();
                    builder.AddSerilog(logger, dispose: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransitHostedService();
                    services.AddMassTransit(busConfigurator =>
                    {
                        busConfigurator.AddConsumer<CustomerChangedConsumer>(typeof(CustomerChangedConsumerDefinition));
                        
                        busConfigurator.UsingAzureServiceBus((context, configurator) =>
                        {
                            configurator.ConfigureEndpoints(context);

                            configurator.Host("");
                            
                            configurator.Message<CustomerChanged>(p => p.SetEntityName("sbt-sample"));
                        });
                    });
                });

            return host;
        }
    }
}
