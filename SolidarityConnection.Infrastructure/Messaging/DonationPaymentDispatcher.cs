using Microsoft.Extensions.Options;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Donations.Commands.CreateDonation;
using SolidarityConnection.Domain.Donation.Models;
using SolidarityConnection.Infrastructure.Options;

namespace SolidarityConnection.Infrastructure.Messaging;

public sealed class DonationPaymentDispatcher(
    IMessagePublisher messagePublisher,
    IOptions<RabbitMqOptions> rabbitMqOptions) : IDonationPaymentDispatcher
{
    public Task DispatchAsync(Donation donation, CancellationToken cancellationToken)
    {
        var donationReceivedEvent = new DonationReceivedEvent(
            donation.Id,
            donation.CampaignId,
            donation.DonorId,
            donation.Amount,
            donation.CreatedAt);

        return messagePublisher.PublishAsync(
            donationReceivedEvent,
            rabbitMqOptions.Value.DonationReceivedQueue,
            cancellationToken);
    }
}
