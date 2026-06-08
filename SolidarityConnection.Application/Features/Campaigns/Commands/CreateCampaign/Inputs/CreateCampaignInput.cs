namespace SolidarityConnection.Application.Features.Campaigns.Commands.CreateCampaign.Inputs;

public class CreateCampaignInput
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TargetAmount { get; set; }

    public CreateCampaignCommand MapToCommand(int managerId)
    {
        return new CreateCampaignCommand(
            Title,
            Description,
            StartDate,
            EndDate,
            TargetAmount,
            managerId);
    }
}
