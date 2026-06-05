namespace SolidarityConnection.Application.Features.Auth.Queries.Login.Outputs;
public class LoginQueryOutput
{
    public string Token { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Role { get; set; } = default!;
}