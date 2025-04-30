using System.Collections.Concurrent;
using System.Text.Json;
using Confluent.Kafka;
using QuestionService.Configurations;
using QuestionService.Models;

namespace QuestionService.Kafka
{
    public class KafkaConsumer
    {
        private readonly IConsumer<Null, string> _consumer;
        private readonly string[] _topics;
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<ValidateUserIDResponse>> _userIDResponses = new();

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
                var consumeResult = _consumer.Consume(cancellationToken);
                var correlationID = consumeResult.Message.Headers
                .FirstOrDefault(h => h.Key == "correlationID")?.GetValueBytes() ?? throw new NullReferenceException("The header is null");
                var correlationIDString = System.Text.Encoding.UTF8.GetString(correlationID);

                switch (consumeResult.Topic)
                {
                    case "validateUserID-request":
                        var message = JsonSerializer.Deserialize<ValidateUserIDResponse>(consumeResult.Message.Value)
                        ?? throw new ArgumentNullException("The givan value is null for validating user");
                        if (_userIDResponses.TryRemove(correlationIDString, out var tcs))
                        {
                            tcs.SetResult(message);
                        }
                        break;
                }
                _consumer.Close();

                if (consumeResult?.Message != null)
                {
                    _logger.LogInformation("Consumed message from topic {Topic}: {Message}", consumeResult.Topic, consumeResult.Message.Value);
                    await messageHandler(consumeResult.Message.Value);
                    _consumer.Commit(consumeResult); // Commit offset after processing
                }
            }
        }
        public Task<ValidateUserIDResponse> WaitForUserIDResponseAsync(string correlationID)
        {
            var tcs = new TaskCompletionSource<ValidateUserIDResponse>();
            _userIDResponses.TryAdd(correlationID, tcs);
            return tcs.Task;
        }
    }

}