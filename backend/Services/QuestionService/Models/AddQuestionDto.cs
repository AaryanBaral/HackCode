using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuestionService.Enums;

namespace QuestionService.Models
{
    public class AddQuestionDto
    {
        public string QuestionId { get; set; }
        public required  string Title {get; set;}
        public required  string Description {get; set;}
        public required  DifficultyEnum Difficulty {get; set;}
        public required  string TimeLimit {get; set;}
        public required  string MemoryLimit {get; set;}
    }
}