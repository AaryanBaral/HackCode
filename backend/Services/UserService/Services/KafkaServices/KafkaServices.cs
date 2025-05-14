using UserService.Kafka.Producer;
using UserService.Model.KafkaModel;
using UserService.Repositories;

namespace UserService.Services.KafkaServices
{
    public class KafkaServices(IAuthRepository repository, KafkaProducer producer)
    {
        private readonly IAuthRepository _repository = repository;
        private readonly KafkaProducer _producer = producer;

        public async Task ValidateUserID(ValidateUserIdRequest request){
            var validateUserIdResponse = new ValidateUserIdResponse(){
                CorrelationID = request.CorrelationID,
                IsValid = await _repository.ValidateUserId(request.UserID),
                Message = "User Id is Valid"
            };
            await _producer.ProduceAsync("validateUserID-request", validateUserIdResponse, request.CorrelationID);
        }
    }
}