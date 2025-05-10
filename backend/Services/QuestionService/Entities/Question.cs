
using QuestionService.Enums;

namespace QuestionService.Entities
{
    public class Question
    {
        public string QuestionId { get; set; } = Guid.NewGuid().ToString();
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DifficultyEnum Difficulty { get; set; }
        public required string TimeLimit { get; set; }
        public required string MemoryLimit { get; set; }
        public required string CreatedBy { get; set; }
        public  bool IsDeleted {get; set;} = false;
        public  DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public  required DateTime UpdatedAt { get; set; }


    }
}