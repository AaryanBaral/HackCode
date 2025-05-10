

namespace QuestionService.Kafka
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