namespace SolidarityConnection.Application.Features.Auth.Commands.DonorRegistration;
public class DonorRegistrationCommand
{
    public DonorRegistrationCommand(string name, string email, string cpf, string password)
    {
        Name = name;
        Email = email;
        Cpf = cpf;
        Password = password;
    }

    public string Name { get; }
    public string Email { get; }
    public string Cpf { get; }
    public string Password { get; }
}
