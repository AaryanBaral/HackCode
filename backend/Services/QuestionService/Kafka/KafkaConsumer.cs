using System.Collections.Concurrent;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
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


        //IOptions<T> is a built-in abstraction in ASP.NET Core used to bind configuration settings 
        // (from appsettings.json, environment variables, etc.) to strongly-typed C# classes.
        //  to use KafkaConfig inside the Ioptions or for the Ioptions class to use the kafka config
        // we add the kafkaConfig class to the DI 
        public KafkaConsumer(IOptions<KafkaConfig> options, string[] topics, ILogger<KafkaConsumer> logger)
        {


            _logger = logger;
            var config = options.Value;
            var bootstrapServers = config.BootstrapServers;
            var groupId = config.ConsumerGroupId;
            _topics = topics;

            if (string.IsNullOrEmpty(bootstrapServers))
                throw new ArgumentNullException(nameof(bootstrapServers), "Kafka:BootstrapServers is not configured.");
            if (string.IsNullOrEmpty(groupId))
                throw new ArgumentNullException(nameof(groupId), "Kafka:GroupId is not configured.");

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
            _consumer.Subscribe(topics);
        }

        public async Task ConsumeAsync(Func<string, Task> messageHandler, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(5));
                if (consumeResult is null) continue;
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

                if (consumeResult?.Message != null)
                {
                    _logger.LogInformation("Consumed message from topic {Topic}: {Message}", consumeResult.Topic, consumeResult.Message.Value);
                    await messageHandler(consumeResult.Message.Value);
                    _consumer.Commit(consumeResult); // Commit offset after processing
                }
            }
            _consumer.Close();
        }
        public Task<ValidateUserIDResponse> WaitForUserIDResponseAsync(string correlationID)
        {
            var tcs = new TaskCompletionSource<ValidateUserIDResponse>();
            _userIDResponses.TryAdd(correlationID, tcs);
            return tcs.Task;
        }
    }
}

