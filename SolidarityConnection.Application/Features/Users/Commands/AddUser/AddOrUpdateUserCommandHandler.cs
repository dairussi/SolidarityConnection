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
        var passwordHash = passwordHasher.Hash(command.Password.Value);

        User user;

        if (!isUpdate)
        {
            if (await userRepository.EmailExistsAsync(command.Email.Value, cancellationToken))
            {
                return ResultData<UserOutput>.Error("E-mail já cadastrado.");
            }
            else
            {

                 user = User.Create(
                    command.Name.Value,
                    command.Email,
                    command.Cpf,
                    passwordHash,
                    command.Role);

                await userRepository.AddAsync(user, cancellationToken);
            }

        }
        else
        {
            user = await userRepository.GetByIdAsync(command.PublicId.Value, cancellationToken);

            user.UpdateDetails(
                command.Name.Value,
                command.Email,
                command.Cpf,
                passwordHash,
                command.Role);

            await userRepository.UpdateAsync(user, cancellationToken);

        }

        
        var userOutput = user.ToOutput();
        return ResultData<UserOutput>.Success(userOutput);
    }
}
