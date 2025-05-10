using Microsoft.AspNetCore.Mvc;
using QuestionService.Models;
using QuestionService.Services;

namespace QuestionService.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionController(IQuestionService questionService) : ControllerBase
    {

        private readonly IQuestionService _questionService = questionService;

        [HttpPost]
        [Route("add")]
        public async  Task<IActionResult> CreateQuestion(AddQuestionDto addQuestionDto){
            string userId = User.FindFirst("UserId")?.Value ?? throw new NullReferenceException("User Id not found");
            await _questionService.AddQuestionAsync(addQuestionDto, userId);
            return Ok(new APIResponse<string>(){Data = "Question created Successfullt"});
        }
    }
}