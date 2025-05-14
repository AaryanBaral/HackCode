

namespace QuestionService.Models
{
    public class ValidateUserIdRequest
    {
        public required string UserID { get; set; }
        public required string CorrelationID { get; set; }
    }
}