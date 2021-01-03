using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using training.message.broker.server.Models;

namespace training.message.broker.server.Demos
{
    public class DuplicateServiceBusMessageBrokerDemo : Demo
    {
        private readonly ServiceBusConfigurations _configurations;
        private readonly ILogger<DuplicateServiceBusMessageBrokerDemo> _logger;

        private QueueClient _queueClient;

        private int _receivedCount = 0;
        private double _billTotal = 0.0;

        public DuplicateServiceBusMessageBrokerDemo(ILogger<DuplicateServiceBusMessageBrokerDemo> logger, ServiceBusConfigurations configurations)
        {
            _logger = logger;
            _configurations = configurations;
        }

        public override async Task RunAsync()
        {
            _queueClient = new QueueClient(_configurations.ConnectionString, _configurations.RfidTagCheckoutQueuePath);

            await ScanAsync();

            _logger.LogInformation("Checkout Console");

            _queueClient.RegisterMessageHandler(HandleMessage, new MessageHandlerOptions(HandleMessageExceptions));

            _logger.LogInformation("Receiving tag read messages...");

            Console.ReadLine();

            // Bill the customer.
            _logger.LogInformation($"Bill customer {_billTotal:c} for {_receivedCount} items.");
        }

        private async Task ScanAsync()
        {
            var orderItems = GetOrderItems();

            PrintOrderTotal(orderItems);

            _logger.LogWarning("Press enter to scan...");

            Console.ReadLine();

            await ScanOrderItems(orderItems);
        }

        private async Task ScanOrderItems(RfidTag[] orderItems)
        {
            var random = new Random(DateTime.Now.Millisecond);


            // Send the order with random duplicate tag reads
            int sentCount = 0;
            int position = 0;

            _logger.LogInformation("Reading tags...");

            while (position < 10)
            {
                RfidTag rfidTag = orderItems[position];

                // Create a new  message from the order item RFID tag.
                var orderJson = JsonConvert.SerializeObject(rfidTag);

                var tagReadMessage = new Message(Encoding.UTF8.GetBytes(orderJson))
                {
                    MessageId = rfidTag.TagId
                };

                // Send the message
                await _queueClient.SendAsync(tagReadMessage);

                _logger.LogInformation($"Sent: { orderItems[position].Product } - MessageId: { tagReadMessage.MessageId }");

                // Randomly cause a duplicate message to be sent.
                if (random.NextDouble() > 0.4) position++;

                sentCount++;

                Thread.Sleep(100);
            }

            _logger.LogInformation($"{sentCount} total tag reads.");
        }

        private void PrintOrderTotal(RfidTag[] orderItems)
        {
            double orderTotal = 0.0;

            foreach (var item in orderItems)
            {
                _logger.LogInformation($"{item.Product} - {item.Price:c}");
                orderTotal += item.Price;
            }

            _logger.LogInformation($"Order value = {orderTotal:c}.");
        }

        private RfidTag[] GetOrderItems()
        {
            _logger.LogInformation("Tag Reader Console");

            // Create a sample order
            RfidTag[] orderItems = new RfidTag[]
            {
                    new RfidTag() { Product = "Ball", Price = 4.99 },
                    new RfidTag() { Product = "Whistle", Price = 1.95 },
                    new RfidTag() { Product = "Bat", Price = 12.99 },
                    new RfidTag() { Product = "Bat", Price = 12.99 },
                    new RfidTag() { Product = "Gloves", Price = 7.99 },
                    new RfidTag() { Product = "Gloves", Price = 7.99 },
                    new RfidTag() { Product = "Cap", Price = 9.99 },
                    new RfidTag() { Product = "Cap", Price = 9.99 },
                    new RfidTag() { Product = "Shirt", Price = 14.99 },
                    new RfidTag() { Product = "Shirt", Price = 14.99 },
            };

            // Display the order data.
            _logger.LogInformation($"Order contains {orderItems.Length} items.");
            return orderItems;
        }

        private Task HandleMessage(Message message, CancellationToken cancellationToken)
        {
            // Process the order message
            var rfidJson = Encoding.UTF8.GetString(message.Body);
            
            var rfidTag = JsonConvert.DeserializeObject<RfidTag>(rfidJson);

            _logger.LogInformation($"{rfidTag}");

            _receivedCount++;

            _billTotal += rfidTag.Price;

            return Task.CompletedTask;
        }

        private Task HandleMessageExceptions(ExceptionReceivedEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Error.");

            return Task.CompletedTask;
        }

        protected override void OnDemoDisposing()
        {
            _queueClient.CloseAsync().Wait();
        }
    }
}
