using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Automatonymous.Activities;
using GreenPipes;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
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

            var nameToPublish = GetNameToPublish();
            while (!string.IsNullOrEmpty(nameToPublish))
            {
                await publisher.Publish<CustomerChanged>(new
                {
                    Id = anotherGuid , Name = nameToPublish
                } , context =>
                {
                    var jsonPayload = JsonSerializer.Serialize(context.Message);
                    using (var hasher = MD5.Create())
                    {
                        var hashBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(jsonPayload));
                        var hashGuid = new Guid(hashBytes);
                        context.MessageId = hashGuid; // <= this is important and ASB duplicate detection uses this property
                    }
                    
                    // context.TryGetPayload(out ServiceBusSendContext payload);
                    // payload.SessionId = guid.ToString();
                    // payload.PartitionKey = guid.ToString();

                    context.SetSessionId(guid.ToString());
                    context.SetPartitionKey(guid.ToString());
                    context.Headers.Set("MessageId" , guid);
                });
                
                nameToPublish = GetNameToPublish();
            }
        }

        private static string GetNameToPublish()
        {
            Console.WriteLine();
            Console.Write("Enter a Name to publish: ");
            var nameToPublish = Console.ReadLine();
            return nameToPublish;
        }
    }
}
