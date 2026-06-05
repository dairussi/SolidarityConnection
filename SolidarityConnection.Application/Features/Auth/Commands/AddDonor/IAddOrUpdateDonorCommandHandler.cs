using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Auth.Queries.Login.Outputs;

namespace SolidarityConnection.Application.Features.Auth.Commands.DonorRegistration;
public interface IAddOrUpdateDonorCommandHandler
{
    Task<ResultData<LoginQueryOutput>> Handle(
    AddOrUpdateDonorCommand command,
    CancellationToken cancellationToken);
}
