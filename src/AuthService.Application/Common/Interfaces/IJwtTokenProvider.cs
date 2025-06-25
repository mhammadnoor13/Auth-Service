using System;

public interface IJwtTokenProvider
{
    string GenerateAccessToken(string userId, string email, string role);
    (string token, DateTime expires) GenerateRefreshToken();
}