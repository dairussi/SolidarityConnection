using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Auth.Queries.Login.Outputs;
using SolidarityConnection.Domain.Enums;
using SolidarityConnection.Domain.Models;

namespace SolidarityConnection.Application.Features.Auth.Commands.ManagerRegistration;
public class ManagerRegistrationCommandHandler : IManagerRegistrationCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public ManagerRegistrationCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<ResultData<LoginQueryOutput>> Handle(
        ManagerRegistrationCommand command,
        CancellationToken cancellationToken)
    {
        if (await _userRepository.EmailExistsAsync(command.Email, cancellationToken))
            return ResultData<LoginQueryOutput>.Error("E-mail já cadastrado.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        // Gestor não tem CPF
        var user = User.Create(
            command.Name,
            command.Email,
            string.Empty,
            passwordHash,
            UserRole.GestorONG);

        await _userRepository.AddAsync(user, cancellationToken);

        var token = _tokenService.GenerateToken(user.Id, user.Role);

        return ResultData<LoginQueryOutput>.Success(new LoginQueryOutput
        {
            Token = token,
            Name = user.Name,
            Role = user.Role
        });
    }
}
