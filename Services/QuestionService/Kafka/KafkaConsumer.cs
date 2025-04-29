using Confluent.Kafka;
using QuestionService.Configurations;

namespace QuestionService.Kafka
{
    public class KafkaConsumer
    {
        private readonly IConsumer<Null, string> _consumer;
        private readonly string[] _topics;
        private readonly ILogger<KafkaConsumer> _logger;

        public KafkaConsumer(KafkaConfig config, string[] topics, ILogger<KafkaConsumer> logger)
        {
            _logger = logger;
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = config.BootstrapServers,
                GroupId = config.ConsumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false // Manual commit for reliability
            };
            _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
            _topics = topics;
            _consumer.Subscribe(topics);
        }

        public async Task ConsumeAsync(Func<string, Task> messageHandler, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(cancellationToken);
                
                if (result?.Message != null)
                {
                    _logger.LogInformation("Consumed message from topic {Topic}: {Message}", result.Topic, result.Message.Value);
                    await messageHandler(result.Message.Value);
                    _consumer.Commit(result); // Commit offset after processing
                }
            }
        }
    }

}