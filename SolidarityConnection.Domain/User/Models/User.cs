using SolidarityConnection.Domain.Common.Enums;
using SolidarityConnection.Domain.Common.Exceptions;
using SolidarityConnection.Domain.Common.Models;
using SolidarityConnection.Domain.User.ValueObjects;

namespace SolidarityConnection.Domain.User.Models;
public class User : BaseModel
{
    private User() { }

    private User(string name, EmailAddress email, CpfValidator cpf, string passwordHash, EUserRole role)
    {
        PublicId = Guid.NewGuid();
        Name = ValidateName(name);
        Email = email;
        Cpf = cpf;
        PasswordHash = passwordHash;
        Role = role;
    }

    public Guid PublicId { get; private set; }
    public string Name { get; private set; } = default!;
    public EmailAddress Email { get; private set; } = default!;
    public CpfValidator Cpf { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public EUserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;

    public static User Create(string name, EmailAddress email, CpfValidator cpf, string passwordHash, EUserRole role)
    {
        return new User(name, email, cpf, passwordHash, role);
    }

    public void UpdateDetails(string name, EmailAddress email, CpfValidator cpf)
    {
        Name = ValidateName(name);
        Email = email;
        Cpf = cpf;
    }

    public void UpdatePassword(string password)
    {
        PasswordHash = password;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void ToggleRole()
    {
        Role = Role == EUserRole.Doador
            ? EUserRole.GestorONG
            : EUserRole.Doador;
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome e requerido.");

        var trimmedName = name.Trim();

        if (!trimmedName.Contains(' '))
            throw new DomainException("Nome deve ser completo.");

        return trimmedName;
    }
}
