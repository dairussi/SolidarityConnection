using SolidarityConnection.Application.Common;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.DeleteCampaign;

public interface IDeleteCampaignCommandHandler
{
    Task<ResultData<bool>> Handle(
        DeleteCampaignCommand command,
        CancellationToken cancellationToken);
}
