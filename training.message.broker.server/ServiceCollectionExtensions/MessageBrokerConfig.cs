using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using training.message.broker.server.Demos;
using training.message.broker.server.Models;

namespace training.message.broker.server.ServiceCollectionExtensions
{
    internal static class MessageBrokerConfig
    {
        internal static void ConfigureMessageBrokers(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceBusConfigurations = new ServiceBusConfigurations();

            configuration.Bind("ServiceBus", serviceBusConfigurations);

            services.AddSingleton(serviceBusConfigurations);

            services.AddSingleton<DemoFactory>();

            services.AddTransient<DefaultDemo>();

            services.AddTransient<SimpleServiceBusMessageBrokerDemo>();
            
            services.AddTransient<ChatServiceBusMessageBrokerDemo>();
        }
    }
}
