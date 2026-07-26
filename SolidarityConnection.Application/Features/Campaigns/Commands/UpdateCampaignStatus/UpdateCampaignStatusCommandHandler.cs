using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Mappers;
using SolidarityConnection.Application.Features.Campaigns.Outputs;
using SolidarityConnection.Domain.Campaign.Enums;
using SolidarityConnection.Domain.Campaign.Exceptions;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.UpdateCampaignStatus;

public sealed class UpdateCampaignStatusCommandHandler(
    ICampaignRepository campaignRepository,
    ICampaignTransparencyWriter transparencyWriter) : IUpdateCampaignStatusCommandHandler
{
    public async Task<ResultData<CampaignOutput>> Handle(
        UpdateCampaignStatusCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var campaign = await campaignRepository.GetByIdAsync(command.CampaignId, cancellationToken);

            if (campaign is null)
            {
                return ResultData<CampaignOutput>.Error("Campanha não encontrada.");
            }

            switch (command.Status)
            {
                case CampaignStatus.Active:
                    campaign.ActivateCampaign();
                    break;

                case CampaignStatus.Paused:
                    campaign.PauseCampaign();
                    break;

                case CampaignStatus.Cancel:
                    campaign.CancelCampaign();
                    break;
                case CampaignStatus.Conclude:
                    campaign.ConcluedCampaing();
                    break;

                default:
                    return ResultData<CampaignOutput>.Error("O status informado para a campanha é inválido.");
            }

            await campaignRepository.UpdateAsync(campaign, cancellationToken);
            await transparencyWriter.UpdateStatusAsync(
                campaign.Id,
                campaign.Status.ToString(),
                cancellationToken);

            return ResultData<CampaignOutput>.Success(campaign.ToOutput());
        }
        catch (CampaignDomainException exception)
        {
            return ResultData<CampaignOutput>.Error(exception.Message);
        }
    }
}