using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Common.Dtos
{
    public record ValidateTokenResponse(
        Guid UserId,
        string Email,
        IEnumerable<string> Roles
    );
}
