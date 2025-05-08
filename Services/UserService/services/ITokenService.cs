using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Entities;

namespace UserService.services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}