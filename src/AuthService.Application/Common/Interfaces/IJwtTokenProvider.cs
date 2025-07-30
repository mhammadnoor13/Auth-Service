using System;
using System.Security.Claims;

public interface IJwtTokenProvider
{
    string GenerateAccessToken(string userId, string email, string role);
    (string token, DateTime expires) GenerateRefreshToken();

    ClaimsPrincipal? ValidateToken(string token);

}