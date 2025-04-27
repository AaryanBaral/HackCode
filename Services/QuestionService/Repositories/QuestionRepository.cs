

using QuestionService.Data;
using QuestionService.Models;

namespace QuestionService.Repositories
{
    public class QuestionRepository(AppDbContext context):IQuestionRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<string> CreateQuestion(AddQuestionDto addQuestionDto){
            
            return "";
        }
    }
}