using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;

namespace SolidarityConnection.Application.Features.Campaigns.Commands.DeleteCampaign;

public sealed class DeleteCampaignCommandHandler(
    ICampaignRepository campaignRepository) : IDeleteCampaignCommandHandler
{

    public async Task<ResultData<bool>> Handle(
        DeleteCampaignCommand command,
        CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetByIdAsync(command.CampaignId, cancellationToken);

        if (campaign is null)
        {
            return ResultData<bool>.Error("Campanha não encontrada.");
        }

        if (campaign.AmountRaised > 0)
        {
            return ResultData<bool>.Error("Não é possível excluir a campanha porque já existe valor doado.");
        }

        await campaignRepository.DeleteAsync(campaign, cancellationToken);

        return ResultData<bool>.Success(true);
    }
}