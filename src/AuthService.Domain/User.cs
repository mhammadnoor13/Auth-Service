using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AuthService.Domain.Entities;

public sealed class User
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public bool EmailConfirmed { get; private set; }

    // Factory – enforces invariants
    public static User Create(string email, string passwordHash)
        => new()
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            EmailConfirmed = false
        };
    public static User Create(string email)
        => new()
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = null!,
            EmailConfirmed = false
        };

    public void ConfirmEmail() => EmailConfirmed = true;
    public void SetPasswordHash(string passwordHash) => PasswordHash = passwordHash;
}
