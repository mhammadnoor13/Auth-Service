using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Common.Exceptions
{
    public class TokenValidationException : Exception
    {
        public TokenValidationException(string message) : base(message) { }
    }
}
