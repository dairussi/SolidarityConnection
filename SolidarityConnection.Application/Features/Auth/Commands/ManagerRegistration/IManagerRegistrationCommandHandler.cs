using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Auth.Queries.Login.Outputs;

namespace SolidarityConnection.Application.Features.Auth.Commands.ManagerRegistration;
public interface IManagerRegistrationCommandHandler
{
    Task<ResultData<LoginQueryOutput>> Handle(
        ManagerRegistrationCommand command,
        CancellationToken cancellationToken);
}
