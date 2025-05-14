using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Model
{
    public class AuthResponseDto
{
    public required string Token { get; set; }
    public required string Role { get; set; }
}

}