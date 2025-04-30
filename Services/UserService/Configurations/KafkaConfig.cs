
namespace UserService.Configurations
{
    public class KafkaConfig
    {
                public  required string BootstrapServers { get; set; }
        public required string ProducerClientId { get; set; }
        public required string ConsumerGroupId { get; set; }
    }
}