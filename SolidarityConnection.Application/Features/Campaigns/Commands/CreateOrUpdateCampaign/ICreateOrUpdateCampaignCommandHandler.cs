using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.CreateOrUpdateCampaign;

public interface ICreateOrUpdateCampaignCommandHandler
{
    Task<ResultData<CampaignOutput>> Handle(
        CreateOrUpdateCampaignCommand command,
        CancellationToken cancellationToken);
}