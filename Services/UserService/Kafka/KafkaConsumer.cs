

using System.Text.Json;
using Confluent.Kafka;
using UserService.Configurations;
using UserService.Models;

namespace UserService.Kafka
{
    public class KafkaConsumer
    {
        private readonly IConsumer<Null, string> _consumer;
        private readonly KafkaProducer  _producer;
        private readonly string[] _topics;
        private readonly ILogger<KafkaConsumer> _logger;
        public KafkaConsumer(KafkaConfig config, string[] topics, ILogger<KafkaConsumer> logger, KafkaProducer producer)
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
            _producer = producer;
            _topics = topics;
            _consumer.Subscribe(topics);

        }

                public async Task ConsumeAsync(Func<string, Task> messageHandler, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                var correlationID = consumeResult.Message.Headers
                .FirstOrDefault(h => h.Key == "correlationID")?.GetValueBytes() ?? throw new NullReferenceException("The header is null");
                var correlationIDString = System.Text.Encoding.UTF8.GetString(correlationID);

                switch (consumeResult.Topic)
                {
                    case "validateUserID-request":
                        var message = JsonSerializer.Deserialize<ValidateUserIDRequest>(consumeResult.Message.Value)
                        ?? throw new NullReferenceException("Null message passed in kafka");
                        // Function call for validating the response
                        bool isValid = true;

                        var resposne = new ValidateUserIDResponse(){
                            IsValid = isValid,
                            CorrelationID = message.CorrelationID,
                            Message = isValid?"User is valid" :"User is not Valid"

                        };
                        await _producer.ProduceAsync("validateUserID-request", resposne, correlationIDString);
                        break;
                }

                if (consumeResult?.Message != null)
                {
                    _logger.LogInformation("Consumed message from topic {Topic}: {Message}", consumeResult.Topic, consumeResult.Message.Value);
                    await messageHandler(consumeResult.Message.Value);
                    _consumer.Commit(consumeResult); // Commit offset after processing
                }
            }
            _consumer.Close();
        }
    }
}