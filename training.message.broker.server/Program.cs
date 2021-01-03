using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

            // Get logger
            var logger = serviceProvider.GetService<ILogger<Program>>();

            var demoFactory = serviceProvider.GetService<DemoFactory>();

            var name = args != null && args.Length > 0 ? args[0] : "Invalid";

            logger.LogInformation($"Demo name: {name}");

            using var demo = demoFactory.Create(name);

            demo.RunAsync().Wait();

            Console.ReadLine();
        }
    }
}
