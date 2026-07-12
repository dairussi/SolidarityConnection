using SolidarityConnection.Domain.Donation.Models;

namespace SolidarityConnection.Application.Common.Interfaces;

public interface IDonationPaymentDispatcher
{
    Task DispatchAsync(Donation donation, CancellationToken cancellationToken);
}
