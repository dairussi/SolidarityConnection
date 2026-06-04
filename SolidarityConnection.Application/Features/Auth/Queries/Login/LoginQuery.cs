namespace SolidarityConnection.Application.Features.Auth.Queries.Login;
public class LoginQuery
{
    public LoginQuery(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public string Email { get; }
    public string Password { get; }
}
