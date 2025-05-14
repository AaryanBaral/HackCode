using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocRunService.Entities
{
    public class Playground
    {
        public string Id { get; set; }
        public string UserId { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string Language { get; set; } = "python"; // default
        public string Output { get; set; } = default!;
        public string Error { get; set; } = default!;
        public bool IsSuccess { get; set; }
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    }
}