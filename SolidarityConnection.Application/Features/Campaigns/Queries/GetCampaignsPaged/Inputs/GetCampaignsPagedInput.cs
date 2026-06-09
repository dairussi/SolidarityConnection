using SolidarityConnection.Application.Features.Campaigns.Filters;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaignsPaged.Inputs;

public class GetCampaignsPagedInput
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public CampaignFilter Filter { get; set; } = new();

    public GetCampaignsPagedQuery MapToQuery()
    {
        return new GetCampaignsPagedQuery(Page, PageSize, Filter);
    }
}
