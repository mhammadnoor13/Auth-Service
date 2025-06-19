using System.Threading.Tasks;
using System.Threading;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken ct);
}