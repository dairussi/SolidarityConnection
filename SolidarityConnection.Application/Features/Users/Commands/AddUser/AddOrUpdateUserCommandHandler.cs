using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Users.Mappers;
using SolidarityConnection.Application.Features.Users.Outputs;
using SolidarityConnection.Domain.User.Models;

namespace SolidarityConnection.Application.Features.Users.Commands.AddUser;
public class AddOrUpdateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher) : IAddOrUpdateUserCommandHandler
{
    public async Task<ResultData<UserOutput>> Handle(
        AddOrUpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        var isUpdate = command.PublicId.HasValue;

        if (!isUpdate)
        {
            if (await userRepository.EmailExistsAsync(command.Email.Value, cancellationToken))
                return ResultData<UserOutput>.Error("E-mail já cadastrado.");
        }

        var passwordHash = passwordHasher.Hash(command.Password.Value);

        var user = User.Create(
            command.Name.Value,
            command.Email,
            command.Cpf,
            passwordHash,
            command.Role);

        await userRepository.AddAsync(user, cancellationToken);

        // O bloco de update fica para implementar depois.
        var userOutput = user.ToOutput();
        return ResultData<UserOutput>.Success(userOutput);
    }
}
