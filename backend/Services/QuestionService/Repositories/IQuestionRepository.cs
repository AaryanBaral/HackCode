using QuestionService.Models;

namespace QuestionService.Repositories
{
    public interface IQuestionRepository
    {
        Task<bool> CreateQuestion(AddQuestionDto addQuestionDto, string userId);
    }
}