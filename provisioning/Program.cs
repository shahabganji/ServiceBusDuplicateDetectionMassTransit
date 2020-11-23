using System.Threading.Tasks;
using Pulumi;

namespace ServiceBusDuplicateDetection.Provisioning
{
    internal class Program
    {
        private static Task<int> Main() => Deployment.RunAsync<MyStack>();
    }
}
