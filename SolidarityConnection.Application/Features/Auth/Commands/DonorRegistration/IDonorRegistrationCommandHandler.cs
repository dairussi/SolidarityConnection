using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Auth.Queries.Login.Outputs;

namespace SolidarityConnection.Application.Features.Auth.Commands.DonorRegistration;
public interface IDonorRegistrationCommandHandler
{
    Task<ResultData<LoginQueryOutput>> Handle(
    DonorRegistrationCommand command,
    CancellationToken cancellationToken);
}
