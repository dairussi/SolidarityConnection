using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Auth.Queries.Login.Outputs;
using SolidarityConnection.Domain.Common.Enums;
using SolidarityConnection.Domain.Users.Models;
using SolidarityConnection.Domain.Users.ValueObjects;

namespace SolidarityConnection.Application.Features.Auth.Commands.DonorRegistration;
public class AddOrUpdateDonorCommandHandler : IAddOrUpdateDonorCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AddOrUpdateDonorCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<ResultData<LoginQueryOutput>> Handle(
        AddOrUpdateDonorCommand command,
        CancellationToken cancellationToken)
    {
        if (await _userRepository.EmailExistsAsync(command.Email.Value, cancellationToken))
            return ResultData<LoginQueryOutput>.Error("E-mail já cadastrado.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password.Value);

        var user = Donor.Create(
            command.Name.Value,
            command.Email.Value,
            command.Cpf.Value,
            passwordHash,
            command.Role;

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
