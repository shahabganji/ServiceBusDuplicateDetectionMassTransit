using System;
using System.Threading;
using System.Threading.Tasks;
using Automatonymous.Activities;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ServiceBusDuplicateDetectionComponents;

namespace ServiceBusDuplicateDetectionProducer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var guid = Guid.NewGuid();
            var anotherGuid = Guid.NewGuid();
            var services = new ServiceCollection();
            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.UsingAzureServiceBus((context, configurator) =>
                {
                    configurator.Host(
                        "");

                    configurator.Message<CustomerChanged>(p => p.SetEntityName("sbt-sample"));
                });
            });

            
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            var publisher = provider.GetRequiredService<IPublishEndpoint>();

            Console.WriteLine();
            Console.Write("Press 'P' to publish: ");
            var publish = Console.ReadKey();
            while (publish.Key == ConsoleKey.P)
            {
                await publisher.Publish<CustomerChanged>(new
                {
                    Id = anotherGuid , Name = "Saeed"
                } , context =>
                {
                    // context.MessageId = Guid.NewGuid(); // <= this is important and ASB duplicate detection uses this property
                    context.Headers.Set("MessageId" , guid);
                });
                
                Console.WriteLine();
                Console.Write("Press 'P' to publish: ");
                publish = Console.ReadKey();
            }
        }
    }
}
