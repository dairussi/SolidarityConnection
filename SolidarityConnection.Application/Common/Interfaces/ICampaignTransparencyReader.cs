using SolidarityConnection.Application.Features.Campaigns.Outputs;

namespace SolidarityConnection.Application.Common.Interfaces;

public interface ICampaignTransparencyReader
{
    Task<(IReadOnlyList<TransparencyCampaignOutput> Items, int TotalCount)> GetActivePagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}