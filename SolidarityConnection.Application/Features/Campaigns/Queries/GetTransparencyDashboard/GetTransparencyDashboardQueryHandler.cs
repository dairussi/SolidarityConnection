using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetTransparencyDashboard;

public sealed class GetTransparencyDashboardQueryHandler(
    ICampaignTransparencyReader transparencyReader) : IGetTransparencyDashboardQueryHandler
{
    public async Task<ResultData<PagedResult<TransparencyCampaignOutput>>> Handle(
        GetTransparencyDashboardQuery query, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await transparencyReader.GetActivePagedAsync(
            query.Page, query.PageSize, cancellationToken);

        var pagedResult = new PagedResult<TransparencyCampaignOutput>
        {
            Items = items.ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };

        return ResultData<PagedResult<TransparencyCampaignOutput>>.Success(pagedResult);
    }
}