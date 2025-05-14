using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using UserService.Config;
using UserService.Model.KafkaModel;
using UserService.Services.KafkaServices;

namespace UserService.Kafka.Consumer
{
    public class KafkaConsumer
    {
        private readonly IConsumer<Null, string> _consumer;
        private readonly string[] _topics;
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly KafkaServices _services;
        // private readonly ConcurrentDictionary<string, TaskCompletionSource<ValidateUserIDResponse>> _userIDResponses = new();

        public KafkaConsumer(IOptions<KafkaConfig> options, string[] topics, ILogger<KafkaConsumer> logger,KafkaServices services)
        {
            _logger = logger;
            var config = options.Value;
            _services = services;
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
                        var message = JsonSerializer.Deserialize<ValidateUserIdRequest>(consumeResult.Message.Value)
                        ?? throw new ArgumentNullException("The given value is null for validating user");
                        await _services.ValidateUserID(message);
                        break;
                }


                if (consumeResult?.Message != null)
                {
                    _logger.LogInformation("Consumed message from topic {Topic}: {Message}", consumeResult.Topic, consumeResult.Message.Value);
                    await messageHandler(consumeResult.Message.Value);
                    _consumer.Commit(consumeResult); // Commit offset after processing
                }
            }
        }
    }
}