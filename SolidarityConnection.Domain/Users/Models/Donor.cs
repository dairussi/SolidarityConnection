using SolidarityConnection.Domain.Common.Enums;

namespace SolidarityConnection.Domain.Users.Models;
public class Donor
{
    private Donor() { }

    private Donor(string name, string email, string cpf, string passwordHash, EUserRole role)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Cpf = cpf;
        PasswordHash = passwordHash;
        Role = role;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Cpf { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public EUserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;

    public static Donor Create(string name, string email, string cpf, string passwordHash, EUserRole role)
    {
        return new Donor(name, email, cpf, passwordHash, role);
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}