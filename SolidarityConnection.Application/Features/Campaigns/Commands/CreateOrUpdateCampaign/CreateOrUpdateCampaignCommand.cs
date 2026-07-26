using SolidarityConnection.Domain.Campaign.Enums;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.CreateOrUpdateCampaign;

public sealed record CreateOrUpdateCampaignCommand(
    Guid? CampaignId,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    decimal TargetAmount,
    CampaignStatus Status,
    int ManagerId);