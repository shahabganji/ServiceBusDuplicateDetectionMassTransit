using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using ServiceBusDuplicateDetection.ConsoleSample.Components;

namespace ServiceBusDuplicateDetection.ConsoleSample.Consumers
{
    class CustomerChangedConsumer : IConsumer<CustomerChanged>
    {
        private static int counter = 0;

        public Task Consume(ConsumeContext<CustomerChanged> context)
        {
            counter++;
            return Task.CompletedTask;
        }
    }

    class CustomerChangedConsumerDefinition : ConsumerDefinition<CustomerChangedConsumer>
    {
        public CustomerChangedConsumerDefinition()
        {
            this.ConcurrentMessageLimit = 1;
            this.EndpointName = "sbs-sample";
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CustomerChangedConsumer> consumerConfigurator)
        {

        }
    }
}
