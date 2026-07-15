using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Donations.Outputs;

namespace SolidarityConnection.Application.Features.Donations.Queries.GetMyTotalsByCampaign;

public sealed class GetMyTotalsByCampaignQueryHandler(
    IDonationRepository donationRepository) : IGetMyTotalsByCampaignQueryHandler
{
    public async Task<ResultData<IReadOnlyList<DonationTotalByCampaignOutput>>> Handle(
        GetMyTotalsByCampaignQuery query,
        CancellationToken cancellationToken)
    {
        var totals = await donationRepository.GetTotalsByDonorIdAsync(query.DonorId, cancellationToken);
        return ResultData<IReadOnlyList<DonationTotalByCampaignOutput>>.Success(totals);
    }
}