using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

namespace training.message.broker.server.ServiceCollectionExtensions
{
    internal static class LoggerConfig
    {
        internal static void ConfigureSerilogLogger(this IServiceCollection services, IConfiguration configuration)
        {
            var logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            services.AddLogging(loggerFactory => loggerFactory.AddSerilog(logger));
        }
    }
}
