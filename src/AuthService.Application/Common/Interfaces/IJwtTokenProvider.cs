using System;

public interface IJwtTokenProvider
{
    string GenerateAccessToken(string userId, string email);
    (string token, DateTime expires) GenerateRefreshToken();
}