

using QuestionService.Models;

namespace QuestionService.Services
{
    public interface IQuestionService
    {
        Task<bool> AddQuestionAsync(AddQuestionDto addQuestionDto, string userID);
    }
}