namespace SolidarityConnection.Application.Features.Auth.Commands.ManagerRegistration.Inputs;
public class ManagerRegistrationInput
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
