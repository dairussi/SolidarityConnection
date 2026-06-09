namespace SolidarityConnection.Application.Features.Auth.Queries.Login.Inputs;

public class LoginQueryInput
{
    public required string Email { get; set; }
    public required string Password { get; set; }

    public LoginQuery MapToQuery()
        => new LoginQuery(Email, Password);
}