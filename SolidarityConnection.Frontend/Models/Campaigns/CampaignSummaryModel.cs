namespace SolidarityConnection.Frontend.Models.Campaigns;

public sealed class CampaignSummaryModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal TargetAmount { get; set; }

    public decimal AmountRaised { get; set; }

    public string Status { get; set; } = string.Empty;
}