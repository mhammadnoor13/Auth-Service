using AuthService.Application.Auth;
using AuthService.Application.Common.Exceptions;
using AuthService.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController, Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _svc;
    private readonly IJwtTokenHandler _tokenHandler;

    public AuthController(IAuthService svc, IJwtTokenHandler tokenHandler)
    {
        _svc = svc;
        _tokenHandler = tokenHandler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand dto, CancellationToken ct)
    {
        var result = await _svc.RegisterAsync(dto, ct);

        if (result.IsSuccess)
            return Ok(result.Value);

        var message = result.Errors.First().Message;

        return Problem(statusCode: 400, detail: message);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand dto, CancellationToken ct)
    {
        var result = await _svc.LoginAsync(dto, ct);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Errors[0]);
    }

    [HttpPost("validate-token")]
    public  IActionResult ValidateToken([FromHeader(Name ="Authorization")] string bearer)
    {
        if (string.IsNullOrWhiteSpace(bearer) ||
            !bearer.StartsWith("Bearer "))
            return BadRequest("Missing or malformed Authorization header.");

        var rawToken = bearer["Bearer ".Length..].Trim();
        try
        {
            var result = _tokenHandler.Handle(rawToken);
            return Ok(result);
        }
        catch (TokenValidationException ex)
        {
            return Unauthorized(ex.Message);
        }

    }
}
