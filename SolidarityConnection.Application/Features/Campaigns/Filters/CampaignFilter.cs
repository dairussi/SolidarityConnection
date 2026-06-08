using SolidarityConnection.Domain.Campaign.Enums;

namespace SolidarityConnection.Application.Features.Campaigns.Filters;

public class CampaignFilter
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public CampaignStatus? Status { get; set; }
}
