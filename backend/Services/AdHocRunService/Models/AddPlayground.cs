using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocRunService.Models
{
    public class AddPlayground
    {
        public required string UserId { get; set; } = default!;
        public required string Code { get; set; } = default!;
        public required string Language { get; set; }
    }
}