using SolidarityConnection.Domain.Campaign.Enums;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.UpdateCampaignStatus.Inputs;

public class UpdateCampaignStatusInput
{
    public CampaignStatus Status { get; set; }
}
