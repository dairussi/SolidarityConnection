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
            return ResultData<CampaignOutput>.Error("Campanha não encontrada.");
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
                return ResultData<CampaignOutput>.Error("A atualização de status da campanha aceita apenas Pausada ou Encerrada.");
        }

        await campaignRepository.UpdateAsync(campaign, cancellationToken);

        return ResultData<CampaignOutput>.Success(campaign.ToOutput());
    }
}
