using SolidarityConnection.Domain.Common.Enums;
using SolidarityConnection.Domain.User.ValueObjects;

namespace SolidarityConnection.Application.Features.Users.Commands.AddUser;
public class AddOrUpdateUserCommand
{
    private AddOrUpdateUserCommand(Guid? publicId, FullName name, EmailAddress email, CpfValidator cpf, RawPassword password, EUserRole role)
    {
        PublicId = publicId;
        Name = name;
        Email = email;
        Cpf = cpf;
        Password = password;
        Role = role;
    }

    public Guid? PublicId { get; }
    public FullName Name { get; }
    public EmailAddress Email { get; }
    public CpfValidator Cpf { get; }
    public RawPassword Password { get; }
    public EUserRole Role { get; set; }

    public static AddOrUpdateUserCommand Create(Guid? publicId, FullName name, EmailAddress email, CpfValidator cpf, RawPassword password, EUserRole role)
    {
        return new AddOrUpdateUserCommand(publicId, name, email, cpf,password, role);
    }
}
