namespace SolidarityConnection.Application.Features.Auth.Commands.ManagerRegistration;
public class ManagerRegistrationCommand
{
    public ManagerRegistrationCommand(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public string Name { get; }
    public string Email { get; }
    public string Password { get; }
}