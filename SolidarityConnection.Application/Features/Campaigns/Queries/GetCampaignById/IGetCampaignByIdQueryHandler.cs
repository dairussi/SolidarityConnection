using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaignById;

public interface IGetCampaignByIdQueryHandler
{
    Task<ResultData<CampaignOutput>> Handle(
        GetCampaignByIdQuery query,
        CancellationToken cancellationToken);
}
