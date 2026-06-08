namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetActiveCampaignsPaged.Inputs;

public class GetActiveCampaignsPagedInput
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetActiveCampaignsPagedQuery MapToQuery()
    {
        return new GetActiveCampaignsPagedQuery(Page, PageSize);
    }
}
