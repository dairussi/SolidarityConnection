using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetTransparencyDashboard;

public interface IGetTransparencyDashboardQueryHandler
{
    Task<ResultData<PagedResult<TransparencyCampaignOutput>>> Handle(
        GetTransparencyDashboardQuery query, CancellationToken cancellationToken);
}