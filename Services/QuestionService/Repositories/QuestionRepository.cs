

using QuestionService.Data;

namespace QuestionService.Repositories
{
    public class QuestionRepository(AppDbContext context):IQuestionRepository
    {
        private readonly AppDbContext _context = context;

        
    }
}