using SolidarityConnection.Application.Features.Users.Commands.AddUser;
using SolidarityConnection.Domain.User.ValueObjects;

namespace SolidarityConnection.Application.Features.Users.Commands.AddUser.Inputs;
public class AddOrUpdateUserInput
{
    public Guid? PublicId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Cpf { get; set; }
    public required string Password { get; set; }

    public AddOrUpdateUserCommand MapToCommand()
    {
        return AddOrUpdateUserCommand.Create(
            PublicId,
            FullName.Create(Name),
            EmailAddress.Create(Email),
            CpfValidator.Create(Cpf),
            RawPassword.Create(Password));
    }
}
