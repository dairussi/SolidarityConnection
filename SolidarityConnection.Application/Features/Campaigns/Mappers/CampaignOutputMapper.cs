using SolidarityConnection.Application.Features.Campaigns.Outputs;
using SolidarityConnection.Domain.Campaign.Models;

namespace SolidarityConnection.Application.Features.Campaigns.Mappers;

public static class CampaignOutputMapper
{
    public static CampaignOutput ToOutput(this Campaign campaign)
    {
        return new CampaignOutput(
            campaign.Id,
            campaign.Title,
            campaign.Description,
            campaign.StartDate,
            campaign.EndDate,
            campaign.TargetAmount,
            campaign.AmountRaised,
            campaign.Status.ToString(),
            campaign.ManagerId);
    }

    public static IReadOnlyList<CampaignOutput> ToOutput(this IEnumerable<Campaign> campaigns)
    {
        return campaigns.Select(ToOutput).ToList();
    }
}
