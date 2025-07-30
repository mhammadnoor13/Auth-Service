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

    public static User Create(string email, string passwordHash)
        => new()
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
        };
    public static User Create(string email)
        => new()
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = null!,
        };

    public void SetPasswordHash(string passwordHash) => PasswordHash = passwordHash;
}
