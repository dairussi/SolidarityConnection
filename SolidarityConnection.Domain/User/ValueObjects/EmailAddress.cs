namespace SolidarityConnection.Domain.User.ValueObjects;
public record EmailAddress
{
    public string Value { get; private set; }
    public EmailAddress()
    {
    }

    private EmailAddress(string email)
    {
        Value = email;
    }

    public static EmailAddress Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail é obrigatório.");

        email = email.Trim();

        int atIndex = email.IndexOf('@');
        if (atIndex < 1)
            throw new ArgumentException("E-mail inválido: precisa conter @.");

        if (email.IndexOf('.', atIndex) < atIndex + 2)
            throw new ArgumentException("E-mail inválido: precisa conter um '.' após o '@'.");

        return new EmailAddress(email);
    }

    public override string ToString() => Value;
}
