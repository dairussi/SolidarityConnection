using System.ComponentModel.DataAnnotations;

namespace SolidarityConnection.Frontend.Models.Auth;

public sealed class RegisterFormModel : IValidatableObject
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return new ValidationResult("Nome é requerido.", [nameof(Name)]);
        }
        else
        {
            var trimmedName = Name.Trim();

            if (trimmedName.Length > 100)
            {
                yield return new ValidationResult("Nome deve ter no máximo 100 caracteres.", [nameof(Name)]);
            }

            if (!trimmedName.Contains(' ', StringComparison.Ordinal))
            {
                yield return new ValidationResult("Nome deve ser completo.", [nameof(Name)]);
            }
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            yield return new ValidationResult("Informe seu e-mail.", [nameof(Email)]);
        }
        else
        {
            var trimmedEmail = Email.Trim();
            var atIndex = trimmedEmail.IndexOf('@');

            if (atIndex < 1)
            {
                yield return new ValidationResult("E-mail inválido: precisa conter @.", [nameof(Email)]);
            }
            else if (trimmedEmail.IndexOf('.', atIndex) < atIndex + 2)
            {
                yield return new ValidationResult("E-mail inválido: precisa conter um '.' após o '@'.", [nameof(Email)]);
            }
        }

        if (string.IsNullOrWhiteSpace(Cpf))
        {
            yield return new ValidationResult("Informe seu CPF.", [nameof(Cpf)]);
        }
        else if (!IsValidCpf(Cpf))
        {
            yield return new ValidationResult("CPF inválido.", [nameof(Cpf)]);
        }

        if (string.IsNullOrEmpty(Password))
        {
            yield return new ValidationResult("Informe sua senha.", [nameof(Password)]);
        }
        else
        {
            if (Password.Length < 8)
            {
                yield return new ValidationResult("A senha deve ter no mínimo 8 caracteres.", [nameof(Password)]);
            }

            if (!Password.Any(char.IsUpper))
            {
                yield return new ValidationResult("A senha deve ter no mínimo 1 letra maiúscula.", [nameof(Password)]);
            }

            if (!Password.Any(character => !char.IsLetterOrDigit(character)))
            {
                yield return new ValidationResult("A senha deve ter no mínimo 1 caractere especial.", [nameof(Password)]);
            }
        }
    }

    public string GetSanitizedCpf()
    {
        return new string(Cpf.Where(char.IsDigit).ToArray());
    }

    private static bool IsValidCpf(string cpf)
    {
        var digits = new string(cpf.Where(char.IsDigit).ToArray());

        if (digits.Length != 11 || digits.Distinct().Count() == 1)
        {
            return false;
        }

        int[] mult1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        var sum = digits
            .Take(9)
            .Select((character, index) => (character - '0') * mult1[index])
            .Sum();

        var remainder = sum % 11;
        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        int[] mult2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];
        sum = digits
            .Take(10)
            .Select((character, index) => (character - '0') * mult2[index])
            .Sum();

        remainder = sum % 11;
        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return digits[9] - '0' == digit1
            && digits[10] - '0' == digit2;
    }
}
