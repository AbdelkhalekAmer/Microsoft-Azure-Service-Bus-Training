using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace training.message.broker.server.ServiceCollectionExtensions
{
    internal static class AppsettingsConfig
    {
        internal static void ConfigureAppsettings(this IServiceCollection services, out IConfiguration configuration)
        {
            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();

            services.AddSingleton(configuration);
        }
    }
}
