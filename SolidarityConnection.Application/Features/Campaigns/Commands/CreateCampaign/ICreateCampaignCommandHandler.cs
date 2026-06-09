using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.CreateCampaign;

public interface ICreateCampaignCommandHandler
{
    Task<ResultData<CampaignOutput>> Handle(
        CreateCampaignCommand command,
        CancellationToken cancellationToken);
}
