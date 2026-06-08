using FCG.Catalog.Application.Common;
using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetActiveCampaignsPaged;

public sealed record GetActiveCampaignsPagedQuery(
    int Page,
    int PageSize);
