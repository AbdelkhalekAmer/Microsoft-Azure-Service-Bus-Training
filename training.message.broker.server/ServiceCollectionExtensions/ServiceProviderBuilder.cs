using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace training.message.broker.server.ServiceCollectionExtensions
{
    internal static class ServiceProviderBuilder
    {
        public static IServiceProvider ConfigureServices() => ConfigureServices(new ServiceCollection());

        private static IServiceProvider ConfigureServices(IServiceCollection services)
        {

            // Add services registeration for configuration file
            services.ConfigureAppsettings(out IConfiguration configuration);

            // Add services registeration for logger
            services.ConfigureSerilogLogger(configuration);

            // Add message brokers
            services.ConfigureMessageBrokers(configuration);

            return services.BuildServiceProvider();
        }
    }
}
