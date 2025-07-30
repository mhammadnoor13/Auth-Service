namespace AuthService.Application.Auth;

// Returned to the client after successful register / login.
public record AuthResult(string AccessToken, string RefreshToken);
