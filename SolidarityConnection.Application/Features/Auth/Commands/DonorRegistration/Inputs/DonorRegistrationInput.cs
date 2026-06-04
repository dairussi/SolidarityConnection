namespace SolidarityConnection.Application.Features.Auth.Commands.DonorRegistration.Inputs;
public class DonorRegistrationInput
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Cpf { get; set; }
    public required string Password { get; set; }
}
