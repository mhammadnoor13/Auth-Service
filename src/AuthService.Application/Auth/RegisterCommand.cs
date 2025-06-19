using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Auth;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Speciality,
    string Email,
    string Password);