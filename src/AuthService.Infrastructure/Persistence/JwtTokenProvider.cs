using System.Security.Cryptography;
using System.Text;
using System;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;



public sealed class JwtTokenProvider : IJwtTokenProvider
{
    private readonly JwtOptions _opt;

    public JwtTokenProvider(IOptions<JwtOptions> opt) => _opt = opt.Value;

    public string GenerateAccessToken(string userId, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role,role)
            },
            expires: DateTime.UtcNow.AddMinutes(_opt.AccessTokenMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string token, DateTime expires) GenerateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var exp = DateTime.UtcNow.AddDays(_opt.RefreshTokenDays);
        return (token, exp);
    }
}

public sealed class JwtOptions
{
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int AccessTokenMinutes { get; init; }
    public int RefreshTokenDays { get; init; }
}