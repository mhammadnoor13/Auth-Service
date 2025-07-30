using System.Threading;
using System.Threading.Tasks;
using AuthService.Application.Common.Interfaces;

namespace AuthService.Infrastructure.Persistence;


public sealed class MongoUnitOfWork : IUnitOfWork
{


    public Task CommitAsync(CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }
}
