using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Mappers;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetActiveCampaignsPaged;

public sealed class GetActiveCampaignsPagedQueryHandler(
    ICampaignRepository campaignRepository) : IGetActiveCampaignsPagedQueryHandler
{

    public async Task<ResultData<PagedResult<CampaignSummaryOutput>>> Handle(
        GetActiveCampaignsPagedQuery query,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await campaignRepository.GetActivePagedAsync(
            query.Page,
            query.PageSize,
            cancellationToken);

        var pagedResult = new PagedResult<CampaignSummaryOutput>
        {
            Items = items.ToSummaryOutput().ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };

        return ResultData<PagedResult<CampaignSummaryOutput>>.Success(pagedResult);
    }
}
