using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

namespace training.message.broker.server.Demos
{
    public class DefaultDemo : Demo
    {
        private readonly ILogger<DefaultDemo> _logger;

        public DefaultDemo(ILogger<DefaultDemo> logger)
        {
            _logger = logger;
        }

        public override Task RunAsync()
        {
            _logger.LogWarning("This is a default demo, the demo you are trying to run is invalid.");

            return Task.CompletedTask;
        }
    }
}
