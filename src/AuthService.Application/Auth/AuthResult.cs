namespace AuthService.Application.Auth;

/// <summary>
/// Returned to the client after successful register / login.
/// </summary>
public record AuthResult(string AccessToken, string RefreshToken);
