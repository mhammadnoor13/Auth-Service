using AuthService.Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Common.Interfaces
{
    public interface IJwtTokenHandler
    {
        ValidateTokenResponse Handle(string token);

    }
}
