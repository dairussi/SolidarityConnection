using SolidarityConnection.Domain.Donation.Models;

namespace SolidarityConnection.Application.Common.Interfaces;

public interface IDonationRepository
{
    Task AddAsync(Donation donation, CancellationToken cancellationToken);
    Task<Donation?> GetByIdAsync(Guid donationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Donation>> ListPendingAsync(CancellationToken cancellationToken);
    Task UpdateAsync(Donation donation, CancellationToken cancellationToken);
}
