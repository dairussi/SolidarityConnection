using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Campaigns.Outputs;
using SolidarityConnection.Domain.Campaign.Enums;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.UpdateCampaignStatus;

public sealed record UpdateCampaignStatusCommand(
    Guid CampaignId,
    CampaignStatus Status);
