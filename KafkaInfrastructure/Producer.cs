using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaInfrastructure
{
    public class Producer
    {
        private ProducerConfig KafkaConfig => new() { BootstrapServers = "localhost:9092" };

        public async Task PublishMessage(string queueName, string message)
        {
            using var producer = new ProducerBuilder<Null, string>(KafkaConfig).Build();
            var result = await producer.ProduceAsync(queueName, new Message<Null, string> { Value = message });
            producer.Flush(new TimeSpan(1000));
        }
    }
}
