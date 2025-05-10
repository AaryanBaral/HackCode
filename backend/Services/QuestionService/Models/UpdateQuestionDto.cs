using QuestionService.Enums;

namespace QuestionService.Models
{
    public class UpdateQuestionDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DifficultyEnum? Difficulty { get; set; }
        public string? TimeLimit { get; set; }
        public string? MemoryLimit { get; set; }
    }
}