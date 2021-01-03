using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using training.message.broker.server.Models;

namespace training.message.broker.server.Demos
{
    public class ChatServiceBusMessageBrokerDemo : Demo
    {
        private readonly ServiceBusConfigurations _configurations;
        
        private readonly ILogger<ChatServiceBusMessageBrokerDemo> _logger;

        private TopicClient _topicClient;

        private SubscriptionClient _subscriptionClient;

        public ChatServiceBusMessageBrokerDemo(ILogger<ChatServiceBusMessageBrokerDemo> logger, ServiceBusConfigurations configurations)
        {
            _logger = logger;
            _configurations = configurations;
        }

        public override async Task RunAsync()
        {
            _logger.LogInformation("Enter name:");

            var userName = Console.ReadLine();

            // Create a management client to manage artifacts
            var manager = new ManagementClient(_configurations.ConnectionString);

            // Create a topic if it does not exist            
            if (!await manager.TopicExistsAsync(_configurations.ChatTopicPath))
            {
                await manager.CreateTopicAsync(_configurations.ChatTopicPath);
            }

            // Create a subscription for the user
            var description = new SubscriptionDescription(_configurations.ChatTopicPath, userName)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
            };

            manager.CreateSubscriptionAsync(description).Wait();


            // Create clients
            _topicClient = new TopicClient(_configurations.ConnectionString, _configurations.ChatTopicPath);

            _subscriptionClient = new SubscriptionClient(_configurations.ConnectionString, _configurations.ChatTopicPath, userName);

            // Create a message pump for receiving messages
            _subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, ExceptionReceivedHandler);

            // Send a message to say you are here
            var helloMessage = new Message(Encoding.UTF8.GetBytes("Has entered the room..."))
            {
                Label = userName
            };

            _topicClient.SendAsync(helloMessage).Wait();

            while (true)
            {
                string text = Console.ReadLine();

                if (text.Equals("exit")) break;

                // Send a chat message
                var chatMessage = new Message(Encoding.UTF8.GetBytes(text))
                {
                    Label = userName
                };

                await _topicClient.SendAsync(chatMessage);
            }

            // Send a message to say you are leaving
            var goodbyeMessage = new Message(Encoding.UTF8.GetBytes("Has left the building..."))
            {
                Label = userName
            };

            _topicClient.SendAsync(goodbyeMessage).Wait();
        }

        private Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Deserialize the message body.
            var text = Encoding.UTF8.GetString(message.Body);

            _logger.LogInformation($"{ message.Label }> { text }");

            return Task.CompletedTask;
        }



        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError(exceptionReceivedEventArgs.Exception, "Error.");

            return Task.CompletedTask;
        }

        protected override void OnDemoDisposing()
        {
            // Close the clients
            _topicClient.CloseAsync().Wait();

            _subscriptionClient.CloseAsync().Wait();
        }
    }
}
