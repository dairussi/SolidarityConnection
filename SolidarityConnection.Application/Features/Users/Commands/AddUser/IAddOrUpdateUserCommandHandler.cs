using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Users.Outputs;

namespace SolidarityConnection.Application.Features.Users.Commands.AddUser;
public interface IAddOrUpdateUserCommandHandler
{
    Task<ResultData<UserOutput>> Handle(
            AddOrUpdateUserCommand command,
            CancellationToken cancellationToken);
}
