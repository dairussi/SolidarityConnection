using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Mappers;
using SolidarityConnection.Application.Features.Campaigns.Outputs;
using SolidarityConnection.Domain.Campaign.Enums;
using SolidarityConnection.Domain.Campaign.Exceptions;
using SolidarityConnection.Domain.Campaign.Models;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.CreateOrUpdateCampaign;

public sealed class CreateOrUpdateCampaignCommandHandler(
    ICampaignRepository campaignRepository,
    ICampaignTransparencyWriter transparencyWriter) : ICreateOrUpdateCampaignCommandHandler
{
    public async Task<ResultData<CampaignOutput>> Handle(
        CreateOrUpdateCampaignCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            Campaign campaign;

            if (command.CampaignId.HasValue)
            {
                var existingCampaign = await campaignRepository.GetByIdAsync(command.CampaignId.Value, cancellationToken);

                if (existingCampaign is null)
                {
                    return ResultData<CampaignOutput>.Error("Campanha não encontrada.");
                }

                campaign = existingCampaign;
                campaign.UpdateDetails(
                    command.Title,
                    command.Description,
                    command.StartDate,
                    command.EndDate,
                    command.TargetAmount);

                var statusResult = ApplyStatus(campaign, command.Status);

                if (!statusResult.IsSuccess)
                {
                    return ResultData<CampaignOutput>.Error(statusResult.ErrorMessage!);
                }

                await campaignRepository.UpdateAsync(campaign, cancellationToken);
            }
            else
            {
                campaign = Campaign.Create(
                    command.Title,
                    command.Description,
                    command.StartDate,
                    command.EndDate,
                    command.TargetAmount,
                    command.ManagerId);

                var statusResult = ApplyStatus(campaign, command.Status);

                if (!statusResult.IsSuccess)
                {
                    return ResultData<CampaignOutput>.Error(statusResult.ErrorMessage!);
                }

                await campaignRepository.AddAsync(campaign, cancellationToken);
            }

            await transparencyWriter.UpsertCampaignAsync(
                campaign.Id,
                campaign.Title,
                campaign.TargetAmount,
                campaign.Status.ToString(),
                cancellationToken);

            return ResultData<CampaignOutput>.Success(campaign.ToOutput());
        }
        catch (CampaignDomainException exception)
        {
            return ResultData<CampaignOutput>.Error(exception.Message);
        }
    }

    private static ResultData<bool> ApplyStatus(Campaign campaign, CampaignStatus status)
    {
        switch (status)
        {
            case CampaignStatus.Active:
                campaign.ActivateCampaign();
                return ResultData<bool>.Success(true);
            case CampaignStatus.Paused:
                campaign.PauseCampaign();
                return ResultData<bool>.Success(true);
            case CampaignStatus.Closed:
                campaign.CloseCampaign();
                return ResultData<bool>.Success(true);
            default:
                return ResultData<bool>.Error("O status informado para a campanha é inválido.");
        }
    }
}