using System.Threading;
using System.Threading.Tasks;
using AuthService.Application.Common.Interfaces;

namespace AuthService.Infrastructure.Persistence;

/// <summary>
///  Unit-Of-Work wrapper for Mongo + MassTransit outbox.
///  If you haven’t enabled the outbox yet, keep the no-op version
///  (just return <see cref="Task.CompletedTask"/>).
/// </summary>
public sealed class MongoUnitOfWork : IUnitOfWork
{
    // Optional: inject IBusOutbox if you enabled cfg.UseMongoDbOutbox(context)
    // private readonly IBusOutbox _outbox;
    // public MongoUnitOfWork(IBusOutbox outbox) => _outbox = outbox;

    public Task CommitAsync(CancellationToken ct = default)
    {
        // With MassTransit outbox:
        // return _outbox.SendCreatedMessages(ct);

        // PLAIN Mongo (auto-commit):
        return Task.CompletedTask;
    }
}
