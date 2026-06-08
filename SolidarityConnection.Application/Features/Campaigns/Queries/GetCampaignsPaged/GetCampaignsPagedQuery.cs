using FCG.Catalog.Application.Common;
using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Campaigns.Filters;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaignsPaged;

public sealed record GetCampaignsPagedQuery(
    int Page,
    int PageSize,
    CampaignFilter Filter);
