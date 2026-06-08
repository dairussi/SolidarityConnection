using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaigns;

public interface IGetCampaignsQueryHandler
{
    Task<ResultData<IReadOnlyList<CampaignOutput>>> Handle(
        GetCampaignsQuery query,
        CancellationToken cancellationToken);
}
