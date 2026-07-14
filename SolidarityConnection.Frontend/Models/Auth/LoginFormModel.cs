using System.ComponentModel.DataAnnotations;

namespace SolidarityConnection.Frontend.Models.Auth;

public class LoginFormModel
{
    [Required(ErrorMessage = "Informe seu e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail valido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe sua senha.")]
    public string Password { get; set; } = string.Empty;
}
