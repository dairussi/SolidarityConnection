using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Users.Outputs;

namespace SolidarityConnection.Application.Features.Users.Commands.ToggleUserRole;

public interface IToggleUserRoleCommandHandler
{
    Task<ResultData<UserOutput>> Handle(
        ToggleUserRoleCommand command,
        CancellationToken cancellationToken);
}
