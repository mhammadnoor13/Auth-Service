// Infrastructure/Security/JwtTokenProvider.cs
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Security
{


    public sealed class JwtOptions
    {
        public required string Key { get; init; }
        public required string Issuer { get; init; }
        public required string Audience { get; init; }
        public int AccessTokenMinutes { get; init; }
        public int RefreshTokenDays { get; init; }
    }

    public sealed class JwtTokenProviderHandle : IJwtTokenProvider
    {
        private readonly JwtOptions _opt;
        private readonly TokenValidationParameters _validationParameters;

        public JwtTokenProviderHandle(IOptions<JwtOptions> options)
        {
            _opt = options.Value;

            var keyBytes = Encoding.UTF8.GetBytes(_opt.Key);
            _validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _opt.Issuer,
                ValidateAudience = true,
                ValidAudience = _opt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }

        public string GenerateAccessToken(string userId, string email, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role,               role)
            };

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_opt.AccessTokenMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string token, DateTime expires) GenerateRefreshToken()
        {
            // Generate a cryptographically strong random string
            var refreshTokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(refreshTokenBytes);
            var expires = DateTime.UtcNow.AddDays(_opt.RefreshTokenDays);

            return (refreshToken, expires);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                // Throws if token is invalid
                var principal = handler.ValidateToken(token, _validationParameters, out _);
                return principal;
            }
            catch (SecurityTokenException)
            {
                return null;
            }
        }
    }
}
