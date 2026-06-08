using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Mappers;
using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaigns;

public sealed class GetCampaignsQueryHandler(
    ICampaignRepository campaignRepository) : IGetCampaignsQueryHandler
{

    public async Task<ResultData<IReadOnlyList<CampaignOutput>>> Handle(
        GetCampaignsQuery query,
        CancellationToken cancellationToken)
    {
        var campaigns = await campaignRepository.ListAsync(cancellationToken);

        return ResultData<IReadOnlyList<CampaignOutput>>.Success(campaigns.ToOutput());
    }
}
