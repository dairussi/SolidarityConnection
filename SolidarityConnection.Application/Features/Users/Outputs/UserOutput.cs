namespace SolidarityConnection.Application.Features.Users.Outputs;
public record UserOutput(
    Guid PublicId,
    string Name,
    string Email,
    string Cpf,
    bool IsActive,
    string Role
);
