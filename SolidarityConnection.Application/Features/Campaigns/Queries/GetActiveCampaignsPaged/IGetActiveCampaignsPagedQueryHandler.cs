using FCG.Catalog.Application.Common;
using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetActiveCampaignsPaged;

public interface IGetActiveCampaignsPagedQueryHandler
{
    Task<ResultData<PagedResult<CampaignSummaryOutput>>> Handle(
        GetActiveCampaignsPagedQuery query,
        CancellationToken cancellationToken);
}
