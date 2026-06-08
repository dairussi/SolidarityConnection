namespace SolidarityConnection.Domain.User.ValueObjects;
public record RawPassword
{
    public RawPassword()
    {
    }

    private RawPassword(string password)
    {
        Value = password;
    }

    public string Value { get; private set; } = default!;

    public static RawPassword Create(string rawInput)
    {
        if (string.IsNullOrEmpty(rawInput))
            throw new ArgumentException("Senha não pode ser vazia.");

        if (rawInput.Length < 8)
            throw new ArgumentException("A senha deve ter no mínimo 8 caracteres.");

        if (!rawInput.Any(char.IsUpper))
            throw new ArgumentException("A senha deve ter no mínimo 1 letra maiúscula.");

        if (!rawInput.Any(charItem => !char.IsLetterOrDigit(charItem)))
            throw new ArgumentException("A senha deve ter no mínimo 1 caractere especial.");

        return new RawPassword(rawInput);
    }
}
