using AuthService.Application.Auth;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _svc;

    public AuthController(IAuthService svc) => _svc = svc;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand dto, CancellationToken ct)
    {
        var result = await _svc.RegisterAsync(dto, ct);

        if (result.IsSuccess)
            return Ok(result.Value);

        // take the first error message (or join them if you want all)
        var message = result.Errors.First().Message;

        // 400 by default; override statusCode if you need a different one
        return Problem(statusCode: 400, detail: message);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand dto, CancellationToken ct)
    {
        var result = await _svc.LoginAsync(dto, ct);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Errors[0]);
    }
}
