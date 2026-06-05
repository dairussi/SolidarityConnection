using SolidarityConnection.Domain.Common.Enums;
using SolidarityConnection.Domain.Users.ValueObjects;


namespace SolidarityConnection.Application.Features.Auth.Commands.DonorRegistration.Inputs;
public class AddOrUpdateDonorInput
{
    public Guid? PublicId { get; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Cpf { get; set; }
    public required string Password { get; set; }
    public required int Role { get; set; }

    public AddOrUpdateDonorCommand MapToCommand()
    {
        return AddOrUpdateDonorCommand.Create(PublicId, FullName.Create(Name), EmailAddress.Create(Email), CpfValidator.Create(Cpf), RawPassword.Create(Password),(EUserRole)Role);
    }
}
