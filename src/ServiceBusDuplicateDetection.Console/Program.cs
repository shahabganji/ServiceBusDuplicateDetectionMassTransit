using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ServiceBusDuplicateDetection.ConsoleSample.Components;
using ServiceBusDuplicateDetection.ConsoleSample.Consumers;

namespace ServiceBusDuplicateDetection.ConsoleSample
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            
            services.AddMassTransit(x =>
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host("connection-string");

                    cfg.Message<CustomerChanged>(z =>
                    {
                        z.SetEntityName("sbt-sample");
                    });

                    // cfg.ConfigurePublish(x => x.(context => {}));

                });

                x.AddConsumersFromNamespaceContaining<CustomerChangedConsumer>();
            });

            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            var publisher = provider.GetRequiredService<IPublishEndpoint>();


            await publisher.Publish<CustomerChanged>(new
            {
                Id = Guid.NewGuid(),
                Name = "Shahab"
            });
        }
    }
}
