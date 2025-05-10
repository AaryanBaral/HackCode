using QuestionService.Enums;

namespace QuestionService.Models
{
    public class ReadListQuesionDto
    {
        public required string QuestionId { get; set; }
        public required string Title { get; set; }
        public required string Difficulty { get; set; }

    }

    public class ReadSingleQuestionDto
    {
        public required string QuestionId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DifficultyEnum Difficulty { get; set; }
        public required string TimeLimit { get; set; }
        public required string MemoryLimit { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}