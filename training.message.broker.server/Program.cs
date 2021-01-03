using Microsoft.Extensions.DependencyInjection;

using System;

using training.message.broker.server.Demos;
using training.message.broker.server.ServiceCollectionExtensions;

namespace training.message.broker.server
{
    internal class Program
    {
        protected Program()
        {

        }

        static void Main(string[] args)
        {
            // Prepare dependency injection container
            var serviceProvider = ServiceProviderBuilder.ConfigureServices();

            var demoFactory = serviceProvider.GetService<DemoFactory>();

            using var demo = demoFactory.Create("");

            demo.RunAsync().Wait();

            Console.ReadLine();
        }
    }
}
