using System.Threading.Tasks;
using System.Threading;
using AuthService.Application.Common.Interfaces;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Persistence;

public sealed class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _col;

    public MongoUserRepository(IMongoDatabase db, ILogger<MongoUserRepository> log)
    {
        _col = db.GetCollection<User>("Users");

        // ensure email is unique – run once on startup
        var emailIdx = Builders<User>.IndexKeys.Ascending(u => u.Email);
        _col.Indexes.CreateOne(
            new CreateIndexModel<User>(emailIdx, new CreateIndexOptions { Unique = true }));
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        _col.Find(u => u.Email == email).FirstOrDefaultAsync(ct);

    public Task<bool> ExistsAsync(string email, CancellationToken ct) =>
        _col.Find(u => u.Email == email).AnyAsync(ct);

    public Task AddAsync(User user, CancellationToken ct) =>
        _col.InsertOneAsync(user, cancellationToken: ct);
}
