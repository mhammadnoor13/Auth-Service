using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Commands;

public record CreateConsultantProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Specialty,
    int Age
);
