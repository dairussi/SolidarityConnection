namespace SolidarityConnection.Domain.Users.ValueObjects;


public record CpfValidator
{
    public string Value { get; private set; }

    private CpfValidator(string cpf)
    {
        Value = cpf;
    }

    public static CpfValidator Create(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF é obrigatório.");

        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (!IsValid(cpf))
            throw new ArgumentException("CPF inválido.");

        return new CpfValidator(cpf);
    }

    private static bool IsValid(string cpf)
    {
        if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
            return false;

        int[] mult1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var sum = cpf
            .Take(9)
            .Select((c, i) => (c - '0') * mult1[i])
            .Sum();

        var remainder = sum % 11;

        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        int[] mult2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        sum = cpf
            .Take(10)
            .Select((c, i) => (c - '0') * mult2[i])
            .Sum();

        remainder = sum % 11;

        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return cpf[9] - '0' == digit1
            && cpf[10] - '0' == digit2;
    }

    public override string ToString() => Value;
}

