using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionService.Models
{
    public class ApiResponse<T>
    {
        public  bool Success {get; set;} = true;
        public required  T Data {get; set;}
    }
}