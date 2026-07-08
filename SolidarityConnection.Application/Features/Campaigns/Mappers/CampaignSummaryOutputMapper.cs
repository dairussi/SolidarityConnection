using SolidarityConnection.Application.Features.Campaigns.Outputs;
using SolidarityConnection.Domain.Campaign.Models;

namespace SolidarityConnection.Application.Features.Campaigns.Mappers;

public static class CampaignSummaryOutputMapper
{
    public static CampaignSummaryOutput ToSummaryOutput(this Campaign campaign)
    {
        return new CampaignSummaryOutput(
            campaign.Id,
            campaign.Title,
            campaign.TargetAmount,
            campaign.AmountRaised
            );
    }

    public static IReadOnlyList<CampaignSummaryOutput> ToSummaryOutput(this IEnumerable<Campaign> campaigns)
    {
        return campaigns.Select(ToSummaryOutput).ToList();
    }
}
