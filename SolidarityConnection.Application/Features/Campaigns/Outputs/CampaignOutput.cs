namespace SolidarityConnection.Application.Features.Campaigns.Outputs;

public record CampaignOutput(
    Guid Id,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    decimal TargetAmount,
    decimal AmountRaised,
    string Status,
    int ManagerId
);
