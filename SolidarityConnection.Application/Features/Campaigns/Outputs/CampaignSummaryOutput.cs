namespace SolidarityConnection.Application.Features.Campaigns.Outputs;

public record CampaignSummaryOutput
(
    Guid Id,
    string Title,
    decimal TargetAmount,
    decimal AmountRaised
);
