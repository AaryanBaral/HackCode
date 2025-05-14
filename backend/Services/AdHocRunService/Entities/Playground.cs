using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocRunService.Entities
{
    public class Playground
    {
        public string Id { get; set; }  = Guid.NewGuid().ToString();
        public required string UserId { get; set; } = default!;
        public required string Code { get; set; } = default!;
        public required string Language { get; set; }
        public string Output { get; set; } = default!;
        public string Error { get; set; } = default!;
        public bool IsSuccess { get; set; }
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    }
}