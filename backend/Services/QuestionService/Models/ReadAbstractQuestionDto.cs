using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuestionService.Enums;

namespace QuestionService.Models
{
    public class ReadAbstractQuestionDto
    {
        public required string QuestionId { get; set; }
        public required string Title { get; set; }
        public required DifficultyEnum Difficulty { get; set; }
    }
}