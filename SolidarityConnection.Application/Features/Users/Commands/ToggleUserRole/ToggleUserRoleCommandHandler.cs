using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Users.Mappers;
using SolidarityConnection.Application.Features.Users.Outputs;

namespace SolidarityConnection.Application.Features.Users.Commands.ToggleUserRole;

public sealed class ToggleUserRoleCommandHandler(
    IUserRepository userRepository) : IToggleUserRoleCommandHandler
{
    public async Task<ResultData<UserOutput>> Handle(
        ToggleUserRoleCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(command.PublicId, cancellationToken);

        if (user is null)
        {
            return ResultData<UserOutput>.Error("Usuário não encontrado.");
        }

        user.ToggleRole();
        await userRepository.UpdateAsync(user, cancellationToken);

        return ResultData<UserOutput>.Success(user.ToOutput());
    }
}
