using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Kafka.Consumer;

namespace UserService.Kafka
{
    public class KafkaConsumerHostedService(KafkaConsumer kafkaConsumer) : BackgroundService
    {
        private readonly KafkaConsumer _kafkaConsumer = kafkaConsumer;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _kafkaConsumer.ConsumeAsync(async message =>
            {
                Console.WriteLine($"Handling extra logic for: {message}");
            }, stoppingToken);
        }
    }
}