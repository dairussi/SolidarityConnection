using SolidarityConnection.Application.Features.Campaigns.Filters;
using SolidarityConnection.Domain.Campaign.Models;

namespace SolidarityConnection.Application.Common.Interfaces;

public interface ICampaignRepository
{
    Task AddAsync(Campaign campaign, CancellationToken cancellationToken);
    Task<Campaign?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Campaign>> ListAsync(CancellationToken cancellationToken);
    Task<(IReadOnlyList<Campaign> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CampaignFilter filter,
        CancellationToken cancellationToken);
    Task<(IReadOnlyList<Campaign> Items, int TotalCount)> GetActivePagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken);
    Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken);
    Task DeleteAsync(Campaign campaign, CancellationToken cancellationToken);
}
