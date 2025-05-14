
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using QuestionService.Configurations;

namespace QuestionService.Kafka
{
    public class KafkaProducer
    {
        private readonly ILogger<KafkaProducer> _logger;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(IOptions<KafkaConfig> options, ILogger<KafkaProducer> logger)
        {
            var config = options.Value;
            _logger = logger;
            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = config.BootstrapServers,
                ClientId = config.ProducerClientId
            };
            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        public async Task ProduceAsync<T>(string topic, T message, string correlationId)
        {
            var kafkaMessage = new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(message),
                Headers = [new Header("correlationID", System.Text.Encoding.UTF32.GetBytes(correlationId))]
            };
            await _producer.ProduceAsync(topic, kafkaMessage);
            _logger.LogInformation("Produced message to topic {Topic}: {Message}", topic, kafkaMessage);
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
        }
    }
}