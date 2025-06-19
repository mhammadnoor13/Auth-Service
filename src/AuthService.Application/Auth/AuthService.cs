using System.Threading.Tasks;
using System.Threading;
using MassTransit;
using AuthService.Application.Common.Interfaces;
using FluentResults;
using AuthService.Domain.Entities;
using Contracts;

namespace AuthService.Application.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHashProvider _hash;
    private readonly IJwtTokenProvider _jwt;
    private readonly IUnitOfWork _uow;
    private readonly IPublishEndpoint _bus;

    public AuthService(
        IUserRepository users,
        IPasswordHashProvider hash,
        IJwtTokenProvider jwt,
        IUnitOfWork uow,
        IPublishEndpoint bus)
    {
        _users = users;
        _hash = hash;
        _jwt = jwt;
        _uow = uow;
        _bus = bus;
    }

    public async Task<Result<AuthResult>> RegisterAsync(RegisterCommand cmd, CancellationToken ct)
    {
        if (await _users.ExistsAsync(cmd.Email, ct))
            return Result.Fail<AuthResult>("Email already exists");

        var user = User.Create(cmd.Email, _hash.Hash(cmd.Password));
        await _users.AddAsync(user, ct);

        // publish outbox event (MassTransit will store it atomically with Mongo)
        await _bus.Publish<IUserRegistered>(new
        {
            user.Id,
            cmd.Email,
            cmd.FirstName,
            cmd.LastName,
            cmd.Speciality
        }, ct);

        await _uow.CommitAsync(ct);

        var access = _jwt.GenerateAccessToken(user.Id.ToString(), user.Email);
        var (refresh, _) = _jwt.GenerateRefreshToken();

        return Result.Ok(new AuthResult(access, refresh));
    }

    public async Task<Result<AuthResult>> LoginAsync(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(cmd.Email, ct);
        if (user is null || !_hash.Verify(cmd.Password, user.PasswordHash))
            return Result.Fail<AuthResult>("Invalid credentials");

        var access = _jwt.GenerateAccessToken(user.Id.ToString(), user.Email);
        var (refresh, _) = _jwt.GenerateRefreshToken();

        return Result.Ok(new AuthResult(access, refresh));
    }
}
