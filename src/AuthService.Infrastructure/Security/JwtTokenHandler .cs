using AuthService.Application.Common.Dtos;
using AuthService.Application.Common.Exceptions;
using AuthService.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Security
{
    public class JwtTokenHandler : IJwtTokenHandler
    {
        private readonly IJwtTokenProvider _provider;

        public JwtTokenHandler(IJwtTokenProvider provider)
        {
            _provider = provider;
        }

        public ValidateTokenResponse Handle(string token)
        {
            var principal = _provider.ValidateToken(token)
                        ?? throw new TokenValidationException("Invalid or expired token");

            var sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? throw new TokenValidationException("Missing sub claim");
            if (!Guid.TryParse(sub, out var userId))
                throw new TokenValidationException("Invalid user ID format");

            var email = principal.FindFirst(ClaimTypes.Email)?.Value
                        ?? throw new TokenValidationException("Missing email claim");

            var roles = principal
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToArray();

            return new ValidateTokenResponse(userId, email, roles);

        }
    }
}
