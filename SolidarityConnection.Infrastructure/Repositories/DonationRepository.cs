using Microsoft.EntityFrameworkCore;
using SolidarityConnection.Application.Common.Interfaces;
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

    public async Task UpdateAsync(Donation donation, CancellationToken cancellationToken)
    {
        context.Donations.Update(donation);
        await context.SaveChangesAsync(cancellationToken);
    }
}
