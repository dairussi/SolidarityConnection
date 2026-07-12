using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Mappers;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaignsPaged;

public sealed class GetCampaignsPagedQueryHandler(
    ICampaignRepository campaignRepository) : IGetCampaignsPagedQueryHandler
{

    public async Task<ResultData<PagedResult<CampaignOutput>>> Handle(
        GetCampaignsPagedQuery query,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await campaignRepository.GetPagedAsync(
            query.Page,
            query.PageSize,
            query.Filter,
            cancellationToken);

        var pagedResult = new PagedResult<CampaignOutput>
        {
            Items = items.ToOutput().ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };

        return ResultData<PagedResult<CampaignOutput>>.Success(pagedResult);
    }
}
