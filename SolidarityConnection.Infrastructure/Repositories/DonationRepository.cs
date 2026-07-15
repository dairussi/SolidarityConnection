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
        var paidTotalsByCampaign = context.Donations
            .AsNoTracking()
            .Where(donation => donation.DonorId == donorId && donation.Status == DonationStatus.Paid)
            .GroupBy(donation => donation.CampaignId)
            .Select(group => new
            {
                CampaignId = group.Key,
                TotalDonated = group.Sum(item => item.Amount)
            });

        return await paidTotalsByCampaign
            .Join(
                context.Campaigns.AsNoTracking(),
                total => total.CampaignId,
                campaign => campaign.Id,
                (total, campaign) => new
                {
                    total.CampaignId,
                    campaign.Title,
                    total.TotalDonated
                })
            .OrderBy(item => item.Title)
            .Select(item => new DonationTotalByCampaignOutput(
                item.CampaignId,
                item.Title,
                item.TotalDonated))
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