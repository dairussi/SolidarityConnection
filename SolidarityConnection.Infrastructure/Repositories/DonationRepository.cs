using Microsoft.EntityFrameworkCore;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Donations.Outputs;
using SolidarityConnection.Domain.Donation.Enums;
using SolidarityConnection.Domain.Donation.Models;
using SolidarityConnection.Infrastructure.Persistence;

namespace SolidarityConnection.Infrastructure.Repositories;

public sealed class DonationRepository(AppDbContext context) : IDonationRepository
{
    public async Task AddAsync(Donation donation, CancellationToken cancellationToken)
    {
        await context.Donations.AddAsync(donation, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Donation?> GetByIdAsync(Guid donationId, CancellationToken cancellationToken)
    {
        return await context.Donations
            .FirstOrDefaultAsync(donation => donation.Id == donationId, cancellationToken);
    }

    public async Task<IReadOnlyList<DonationTotalByCampaignOutput>> GetTotalsByDonorIdAsync(
        int donorId,
        CancellationToken cancellationToken)
    {
        return await context.Donations
            .AsNoTracking()
            .Where(donation => donation.DonorId == donorId)
            .Join(
                context.Campaigns.AsNoTracking(),
                donation => donation.CampaignId,
                campaign => campaign.Id,
                (donation, campaign) => new
                {
                    donation.CampaignId,
                    campaign.Title,
                    donation.Amount
                })
            .GroupBy(item => new { item.CampaignId, item.Title })
            .Select(group => new DonationTotalByCampaignOutput(
                group.Key.CampaignId,
                group.Key.Title,
                group.Sum(item => item.Amount)))
            .OrderBy(item => item.CampaignTitle)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Donation>> ListPendingAsync(CancellationToken cancellationToken)
    {
        return await context.Donations
            .AsNoTracking()
            .Where(donation => donation.Status == DonationStatus.Pending)
            .OrderBy(donation => donation.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Donation donation, CancellationToken cancellationToken)
    {
        context.Donations.Update(donation);
        await context.SaveChangesAsync(cancellationToken);
    }
}