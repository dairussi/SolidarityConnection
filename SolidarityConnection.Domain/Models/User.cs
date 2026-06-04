namespace SolidarityConnection.Domain.Models;
public class User
{
    private User() { }

    private User(string name, string email, string cpf, string passwordHash, string role)
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
    public string Role { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    public static User Create(string name, string email, string cpf, string passwordHash, string role)
    {
        return new User(name, email, cpf, passwordHash, role);
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}