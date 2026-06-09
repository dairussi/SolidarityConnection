using SolidarityConnection.Application.Common;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.DeleteCampaign;

public sealed record DeleteCampaignCommand(Guid CampaignId);
