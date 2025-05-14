using Microsoft.AspNetCore.Mvc;
using QuestionService.Data;
using QuestionService.Models;
using QuestionService.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IQuestionService _questionService;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext context, IQuestionService questionService, IConfiguration Configuration)
        {
            _context = context;
            _questionService = questionService;
            _configuration = Configuration;
        }

        [HttpPost("add")]

        public async Task<IActionResult> AddQuestion(AddQuestionDto addQuestionDto)
        {
            var userId = User.FindFirst("UserId")?.Value ?? throw new NullReferenceException("Token is not Valid");
            await _questionService.AddQuestionAsync(addQuestionDto, userId);
            return Ok(new APIResponse<string>(){
                Data = "Question Addded successfully"
            });
        }

    }

}