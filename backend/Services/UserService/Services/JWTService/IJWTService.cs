using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UserService.Entities;

namespace UserService.Services.JWTService
{
    public interface IJWTService
    {
         string GenerateJwtToken(User user);
    }
}