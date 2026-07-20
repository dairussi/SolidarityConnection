using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Mappers;
using SolidarityConnection.Application.Features.Campaigns.Outputs;
using SolidarityConnection.Domain.Campaign.Enums;
using SolidarityConnection.Domain.Campaign.Models;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.CreateCampaign;

public sealed class CreateCampaignCommandHandler(
    ICampaignRepository campaignRepository,
    ICampaignTransparencyWriter transparencyWriter) : ICreateCampaignCommandHandler
{
    public async Task<ResultData<CampaignOutput>> Handle(
        CreateCampaignCommand command,
        CancellationToken cancellationToken)
    {
        var campaign = Campaign.Create(
            command.Title, command.Description, command.StartDate,
            command.EndDate, command.TargetAmount, command.ManagerId);

        switch (command.Status)
        {
            case CampaignStatus.Paused:
                campaign.PauseCampaign();
                break;
            case CampaignStatus.Closed:
                campaign.CloseCampaign();
                break;
        }

        await campaignRepository.AddAsync(campaign, cancellationToken);

        await transparencyWriter.UpsertCampaignAsync(
            campaign.Id, campaign.Title, campaign.TargetAmount, campaign.Status.ToString(), cancellationToken);

        return ResultData<CampaignOutput>.Success(campaign.ToOutput());
    }
}