using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Mappers;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaignById;

public sealed class GetCampaignByIdQueryHandler(
    ICampaignRepository campaignRepository) : IGetCampaignByIdQueryHandler
{

    public async Task<ResultData<CampaignOutput>> Handle(
        GetCampaignByIdQuery query,
        CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetByIdAsync(query.CampaignId, cancellationToken);

        if (campaign is null)
        {
            return ResultData<CampaignOutput>.Error("Campaign not found.");
        }

        return ResultData<CampaignOutput>.Success(campaign.ToOutput());
    }
}
