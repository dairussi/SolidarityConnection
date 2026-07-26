using SolidarityConnection.Domain.Campaign.Enums;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.CreateOrUpdateCampaign.Inputs;

public class CreateOrUpdateCampaignInput
{
    public Guid? Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TargetAmount { get; set; }
    public CampaignStatus Status { get; set; } = CampaignStatus.Active;

    public CreateOrUpdateCampaignCommand MapToCommand(int managerId)
    {
        return new CreateOrUpdateCampaignCommand(
            Id,
            Title,
            Description,
            StartDate,
            EndDate,
            TargetAmount,
            Status,
            managerId);
    }
}