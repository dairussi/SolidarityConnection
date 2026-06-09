using FCG.Catalog.Application.Common;
using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaignsPaged;

public interface IGetCampaignsPagedQueryHandler
{
    Task<ResultData<PagedResult<CampaignOutput>>> Handle(
        GetCampaignsPagedQuery query,
        CancellationToken cancellationToken);
}
