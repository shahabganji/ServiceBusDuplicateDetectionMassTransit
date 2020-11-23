using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using ServiceBusDuplicateDetection.Consumer.Components;

namespace ServiceBusDuplicateDetection.Consumer.Consumers
{
    public class CustomerChangedConsumer : IConsumer<CustomerChanged>
    {
        private static int _counter = 0;

        public Task Consume(ConsumeContext<CustomerChanged> context)
        {
            _counter++;
            Console.WriteLine(_counter);
            return Task.CompletedTask;
        }
    }

    public class CustomerChangedConsumerDefinition : ConsumerDefinition<CustomerChangedConsumer>
    {
        public CustomerChangedConsumerDefinition()
        {
            EndpointDefinition = new ConsumerEndpointDefinition<CustomerChangedConsumer>(
                new EndpointSettings<IEndpointDefinition<CustomerChangedConsumer>>
                {
                    Name = $"sbs-sample",
                    IsTemporary = false,
                    PrefetchCount = 1,
                    ConcurrentMessageLimit = 1
                });
            this.ConcurrentMessageLimit = 1;
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<CustomerChangedConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseInMemoryOutbox();
        }
    }
}
