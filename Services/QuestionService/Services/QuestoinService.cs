using QuestionService.Kafka;
using QuestionService.Models;
using QuestionService.Repositories;

namespace QuestionService.Services
{
    public class QuestoinService(KafkaProducer producer, KafkaConsumer responseConsumer, IQuestionRepository repository)
    {
        private  readonly KafkaProducer _producer = producer;
        private  readonly KafkaConsumer _responseConsumer = responseConsumer;
        private  readonly IQuestionRepository _repository = repository;
        public async Task<bool> AddQuestionAsync(AddQuestionDto addQuestionDto, string userID)

        {
            // Generate unique correlation ID
            var correlationID = Guid.NewGuid().ToString() ?? throw new NullReferenceException("the guid is generated null");

            // Send validation request
            var request = new ValidateUserIdRequest { UserID = userID, CorrelationID = correlationID };
            await _producer.ProduceAsync("validateUserID-request", request, correlationID);

            // Wait for response
            var response = await _responseConsumer.WaitForUserIDResponseAsync(correlationID);

            if (!response.IsValid)
            {
                throw new KeyNotFoundException($"given key not found {response.Message}");
            }

            
            await _repository.CreateQuestion(addQuestionDto, userID);

            return true;
        }
    }
}