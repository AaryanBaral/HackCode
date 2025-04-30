

namespace UserService.Models
{
    public class ValidateUserIDRequest
    {
        
           public required string UserID { get; set; }
        public required string CorrelationID { get; set; }
    }
}