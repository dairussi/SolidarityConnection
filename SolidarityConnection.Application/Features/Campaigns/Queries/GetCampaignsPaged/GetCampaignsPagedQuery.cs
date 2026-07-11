using SolidarityConnection.Application.Features.Campaigns.Filters;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaignsPaged;

public sealed record GetCampaignsPagedQuery(
    int Page,
    int PageSize,
    CampaignFilter Filter);
