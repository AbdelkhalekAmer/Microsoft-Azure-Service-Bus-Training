using System;

namespace training.message.broker.server.Demos
{
    public class DemoFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DemoFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Demo Create(string name) => name switch
        {
            "simple" => _serviceProvider.GetService(typeof(SimpleServiceBusMessageBrokerDemo)) as Demo,
            "chat" => _serviceProvider.GetService(typeof(ChatServiceBusMessageBrokerDemo)) as Demo,
            "duplicate" => _serviceProvider.GetService(typeof(DuplicateServiceBusMessageBrokerDemo)) as Demo,
            _ => _serviceProvider.GetService(typeof(DefaultDemo)) as Demo
        };
    }
}
