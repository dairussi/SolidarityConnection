using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Auth.Queries.Login.Outputs;
using SolidarityConnection.Domain.Enums;
using SolidarityConnection.Domain.Models;
using SolidarityConnection.Domain.Validators;

namespace SolidarityConnection.Application.Features.Auth.Commands.DonorRegistration;
public class DonorRegistrationCommandHandler : IDonorRegistrationCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public DonorRegistrationCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<ResultData<LoginQueryOutput>> Handle(
        DonorRegistrationCommand command,
        CancellationToken cancellationToken)
    {
        if (!CpfValidator.IsValid(command.Cpf))
            return ResultData<LoginQueryOutput>.Error("CPF inválido.");

        if (await _userRepository.EmailExistsAsync(command.Email, cancellationToken))
            return ResultData<LoginQueryOutput>.Error("E-mail já cadastrado.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        var user = User.Create(
            command.Name,
            command.Email,
            command.Cpf,
            passwordHash,
            UserRole.Doador);

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
