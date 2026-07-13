using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Donations.Outputs;

namespace SolidarityConnection.Application.Features.Donations.Queries.GetMyTotalsByCampaign;

public interface IGetMyTotalsByCampaignQueryHandler
{
    Task<ResultData<IReadOnlyList<DonationTotalByCampaignOutput>>> Handle(
        GetMyTotalsByCampaignQuery query,
        CancellationToken cancellationToken);
}