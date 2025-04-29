using QuestionService.Data;
using QuestionService.Entities;
using QuestionService.Mappers;
using QuestionService.Models;

namespace QuestionService.Repositories
{
    public class QuestionRepository(AppDbContext context):IQuestionRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<string> CreateQuestion(AddQuestionDto addQuestionDto, string userId){
            // user id is already validated 
            Question question = QuestionMapper.ToQueston(addQuestionDto,userId);
            
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
            return "";
        }
    }
}