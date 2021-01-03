using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

using System.Text;
using System.Threading;
using System.Threading.Tasks;

using training.message.broker.server.Models;

namespace training.message.broker.server.Demos
{
    public class SimpleServiceBusMessageBrokerDemo : Demo
    {
        private readonly ServiceBusConfigurations _configurations;
        private readonly ILogger<SimpleServiceBusMessageBrokerDemo> _logger;

        private QueueClient _queueClient;

        public SimpleServiceBusMessageBrokerDemo(ILogger<SimpleServiceBusMessageBrokerDemo> logger, ServiceBusConfigurations configurations)
        {
            _logger = logger;
            _configurations = configurations;
        }

        public override async Task RunAsync()
        {
            _queueClient = new QueueClient(_configurations.ConnectionString, _configurations.SimpleQueuePath);

            _queueClient.RegisterMessageHandler(HandleReceivedMessage, HandleReceivedMessageError);

            var message = $"Simple message from {nameof(SimpleServiceBusMessageBrokerDemo)}.";

            var serviceBusMessage = new Message(Encoding.UTF8.GetBytes(message));

            _logger.LogWarning($"Sending message: {message}");

            await _queueClient.SendAsync(serviceBusMessage);
        }

        private Task HandleReceivedMessageError(ExceptionReceivedEventArgs args)
        {
            _logger.LogError(args.Exception, "error");

            return Task.CompletedTask;
        }

        private Task HandleReceivedMessage(Message message, CancellationToken token)
        {
            _logger.LogInformation($"Received message: {Encoding.UTF8.GetString(message.Body)}");

            return Task.CompletedTask;
        }

        protected override void OnDemoDisposing()
        {
            _queueClient.CloseAsync().Wait();
        }
    }
}
