namespace SolidarityConnection.Frontend.Models.Auth;

public sealed class UserSession
{
    public string Token { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

    public bool IsAdmin => string.Equals(Role, "GestorONG", StringComparison.OrdinalIgnoreCase);

    public static UserSession Guest => new();
}