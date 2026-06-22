using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Users.Mappers;
using SolidarityConnection.Application.Features.Users.Outputs;
using SolidarityConnection.Domain.Common.Enums;
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
        User user;

        if (!command.PublicId.HasValue)
        {
            if (await userRepository.EmailExistsAsync(command.Email.Value, cancellationToken))
                return ResultData<UserOutput>.Error("E-mail já cadastrado.");

            if (await userRepository.CpfExistsAsync(command.Cpf.Value, cancellationToken))
                return ResultData<UserOutput>.Error("CPF já cadastrado.");

            var passwordHash = passwordHasher.Hash(command.Password.Value);

            user = User.Create(command.Name.Value, command.Email, command.Cpf, passwordHash, EUserRole.Doador);
            await userRepository.AddAsync(user, cancellationToken);
        }
        else
        {
            if (await userRepository.EmailExistsAsync(command.Email.Value, cancellationToken, command.PublicId))
                return ResultData<UserOutput>.Error("E-mail já cadastrado por outro usuário.");

            if (await userRepository.CpfExistsAsync(command.Cpf.Value, cancellationToken, command.PublicId))
                return ResultData<UserOutput>.Error("CPF já cadastrado por outro usuário.");

            var existingUser = await userRepository.GetByIdAsync(command.PublicId.Value, cancellationToken);

            if (existingUser is null)
                return ResultData<UserOutput>.Error("Usuário não encontrado.");

            user = existingUser;
            user.UpdateDetails(command.Name.Value, command.Email, command.Cpf);
            await userRepository.UpdateAsync(user, cancellationToken);
        }

        return ResultData<UserOutput>.Success(user.ToOutput());
    }
}
