
using System.Text.Json;
using Confluent.Kafka;
using QuestionService.Configurations;

namespace QuestionService.Kafka
{
    public class KafkaProducer
    {
        private readonly ILogger<KafkaProducer> _logger;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(KafkaConfig config, ILogger<KafkaProducer> logger){
            _logger = logger;
            var producerCongig = new ProducerConfig(){
                BootstrapServers = config.BootstrapServers,
                ClientId = config.ProducerClientId
            };
            _producer = new ProducerBuilder<Null, string>(producerCongig).Build();
        }

        public async Task ProduceAsync(string topic, string message){
            var serializedMessage = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(topic,new Message<Null, string>{Value = serializedMessage});
            _logger.LogInformation("Produced message to topic {Topic}: {Message}", topic, serializedMessage);
        }
        
    }
}