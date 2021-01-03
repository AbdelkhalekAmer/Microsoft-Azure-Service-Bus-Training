using System;

namespace training.message.broker.server.Models
{
    public class RfidTag
    {
        public string TagId { get; } = Guid.NewGuid().ToString();

        public string Product { get; set; }

        public double Price { get; set; }

        public override string ToString() => $"Product:{ Product }\tPrice:${ Price }";
    }
}
