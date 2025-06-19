using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

public sealed class AspNetPasswordHashProvider : IPasswordHashProvider
{
    private readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();

    public string Hash(string pwd) => _hasher.HashPassword(null!, pwd);

    public bool Verify(string pwd, string hashed) =>
        _hasher.VerifyHashedPassword(null!, hashed, pwd) == PasswordVerificationResult.Success;
}
