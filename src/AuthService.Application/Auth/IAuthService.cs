using FluentResults;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Auth;

public interface IAuthService
{
    Task<Result<AuthResult>> RegisterAsync(RegisterCommand command, CancellationToken ct);
    Task<Result<AuthResult>> LoginAsync(LoginCommand command, CancellationToken ct);

    // Add later if you implement refresh-tokens
    // Task<Result<AuthResult>> RefreshAsync(RefreshTokenCommand command, CancellationToken ct);
}
