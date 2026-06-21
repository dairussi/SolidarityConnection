using SolidarityConnection.Domain.User.ValueObjects;

namespace SolidarityConnection.Application.Features.Users.Commands.AddUser;
public class AddOrUpdateUserCommand
{
    private AddOrUpdateUserCommand(Guid? publicId, FullName name, EmailAddress email, CpfValidator cpf, RawPassword password)
    {
        PublicId = publicId;
        Name = name;
        Email = email;
        Cpf = cpf;
        Password = password;
    }

    public Guid? PublicId { get; }
    public FullName Name { get; }
    public EmailAddress Email { get; }
    public CpfValidator Cpf { get; }
    public RawPassword Password { get; }

    public static AddOrUpdateUserCommand Create(
        Guid? publicId,
        FullName name,
        EmailAddress email,
        CpfValidator cpf,
        RawPassword password)
    {
        return new AddOrUpdateUserCommand(publicId, name, email, cpf, password);
    }
}
