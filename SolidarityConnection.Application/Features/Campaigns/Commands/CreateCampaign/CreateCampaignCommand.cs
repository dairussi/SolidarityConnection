namespace SolidarityConnection.Application.Features.Campaigns.Commands.CreateCampaign;

public sealed record CreateCampaignCommand(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    decimal TargetAmount,
    int ManagerId);
