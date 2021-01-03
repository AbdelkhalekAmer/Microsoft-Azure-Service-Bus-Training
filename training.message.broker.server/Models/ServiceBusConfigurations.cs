namespace training.message.broker.server.Models
{
    public class ServiceBusConfigurations
    {
        public string ConnectionString { get; set; }
        public string SimpleQueuePath { get; set; }
        public string ChatTopicPath { get; set; }
        public string RfidTagCheckoutQueuePath { get; set; }
    }
}
