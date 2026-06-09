using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.UpdateCampaignStatus;

public interface IUpdateCampaignStatusCommandHandler
{
    Task<ResultData<CampaignOutput>> Handle(
        UpdateCampaignStatusCommand command,
        CancellationToken cancellationToken);
}
