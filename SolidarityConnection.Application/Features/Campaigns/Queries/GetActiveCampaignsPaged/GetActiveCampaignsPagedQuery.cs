namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetActiveCampaignsPaged;

public sealed record GetActiveCampaignsPagedQuery(
    int Page,
    int PageSize);
