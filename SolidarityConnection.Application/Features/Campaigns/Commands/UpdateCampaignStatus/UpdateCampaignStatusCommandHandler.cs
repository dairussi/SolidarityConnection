using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Mappers;
using SolidarityConnection.Application.Features.Campaigns.Outputs;
using SolidarityConnection.Domain.Campaign.Enums;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.UpdateCampaignStatus;

public sealed class UpdateCampaignStatusCommandHandler(
    ICampaignRepository campaignRepository) : IUpdateCampaignStatusCommandHandler
{

    public async Task<ResultData<CampaignOutput>> Handle(
        UpdateCampaignStatusCommand command,
        CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetByIdAsync(command.CampaignId, cancellationToken);

        if (campaign is null)
        {
            return ResultData<CampaignOutput>.Error("Campaign not found.");
        }

        switch (command.Status)
        {
            case CampaignStatus.Paused:
                campaign.PauseCampaign();
                break;

            case CampaignStatus.Closed:
                campaign.CloseCampaign();
                break;

            default:
                return ResultData<CampaignOutput>.Error("Campaign status update only supports Paused or Closed.");
        }

        await campaignRepository.UpdateAsync(campaign, cancellationToken);

        return ResultData<CampaignOutput>.Success(campaign.ToOutput());
    }
}
