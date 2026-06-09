using Microsoft.EntityFrameworkCore;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Filters;
using SolidarityConnection.Domain.Campaign.Enums;
using SolidarityConnection.Domain.Campaign.Models;
using SolidarityConnection.Infrastructure.Persistence;

namespace SolidarityConnection.Infrastructure.Repositories;

public sealed class CampaignRepository(AppDbContext context) : ICampaignRepository
{
    public async Task AddAsync(Campaign campaign, CancellationToken cancellationToken)
    {
        await context.Campaigns.AddAsync(campaign, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Campaign?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Campaigns
            .FirstOrDefaultAsync(campaign => campaign.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Campaign>> ListAsync(CancellationToken cancellationToken)
    {
        return await context.Campaigns
            .AsNoTracking()
            .OrderByDescending(campaign => campaign.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Campaign> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CampaignFilter filter,
        CancellationToken cancellationToken)
    {
        var query = ApplyFilter(
                context.Campaigns.AsNoTracking(),
                filter)
            .OrderByDescending(campaign => campaign.StartDate);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<Campaign> Items, int TotalCount)> GetActivePagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = context.Campaigns
            .AsNoTracking()
            .Where(campaign => campaign.Status == CampaignStatus.Active)
            .OrderByDescending(campaign => campaign.StartDate);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken)
    {
        context.Campaigns.Update(campaign);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Campaign campaign, CancellationToken cancellationToken)
    {
        context.Campaigns.Remove(campaign);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Campaign> ApplyFilter(
        IQueryable<Campaign> query,
        CampaignFilter filter)
    {
        if (filter.DateFrom.HasValue)
        {
            query = query.Where(campaign => campaign.StartDate >= filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue)
        {
            query = query.Where(campaign => campaign.EndDate <= filter.DateTo.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(campaign => campaign.Status == filter.Status.Value);
        }

        return query;
    }
}
