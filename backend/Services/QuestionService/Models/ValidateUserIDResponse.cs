namespace QuestionService.Models
{
    public class ValidateUserIDResponse
    {
        public bool IsValid { get; set; }
        public required string Message { get; set; }
        public required  string CorrelationID { get; set; }
    }
}