using System;
using System.Threading.Tasks;
using System.Threading;
using AuthService.Domain.Entities;

namespace AuthService.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<bool> ExistsAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}