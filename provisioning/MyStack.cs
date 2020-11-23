using Pulumi;
using Pulumi.Azure.Core;
using Pulumi.Azure.ServiceBus;

namespace ServiceBusDuplicateDetection.Provisioning
{
    internal  class MyStack : Stack
    {

        public MyStack()
        {
            ProvisioningServiceBus();
        }

        private void ProvisioningServiceBus()
        {
            var resourceGroup = new ResourceGroup("shahab");

            var serviceBusNamespace = new Namespace(
                "sbns-sample", new NamespaceArgs
                {
                    ResourceGroupName = resourceGroup.Name,
                    Sku = "Standard",
                    Tags =
                    {
                        {"tech", "service bus"},
                        {"feature", "duplicate detection"},
                    },
                });

            var topic = new Topic("sbt-sample",
                new TopicArgs
                {
                    Name = "sbt-sample",
                    ResourceGroupName = resourceGroup.Name,
                    NamespaceName = serviceBusNamespace.Name,
                    EnablePartitioning = true, 
                    RequiresDuplicateDetection = true , 
                    // DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(2).ToString() // default is 10 min
                });

            var sampleSubscriptionOnFirstTopic =
                new Subscription("sbs-sample",
                    new SubscriptionArgs
                    {
                        Name = "sbs-sample",
                        ResourceGroupName = resourceGroup.Name,
                        NamespaceName = serviceBusNamespace.Name,
                        TopicName = topic.Name,
                        MaxDeliveryCount = 5,
                    });
        }
    }
}
