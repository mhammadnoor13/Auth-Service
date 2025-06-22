using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Commands;

public record CreateUserCommand(
    Guid RegistrationId,
    string Email,
    string Password
);
