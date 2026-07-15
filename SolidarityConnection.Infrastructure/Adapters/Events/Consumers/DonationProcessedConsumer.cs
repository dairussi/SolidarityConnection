using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolidarityConnection.Domain.Donation.Enums;
using SolidarityConnection.Infrastructure.Messaging.Events;
using SolidarityConnection.Infrastructure.Persistence;

namespace SolidarityConnection.Infrastructure.Adapters.Events.Consumers;

public sealed class DonationProcessedConsumer(
    AppDbContext context,
    ILogger<DonationProcessedConsumer> logger)
{
    public async Task ConsumeAsync(
        DonationProcessedEvent donationProcessedEvent,
        CancellationToken cancellationToken)
    {
        var donationStatus = ParseStatus(donationProcessedEvent.Status);

        logger.LogInformation(
            "Recebido DonationProcessedEvent - DonationId: {DonationId}, Status: {Status}, CampaignId: {CampaignId}, DonorId: {DonorId}",
            donationProcessedEvent.DonationId,
            donationStatus,
            donationProcessedEvent.CampaignId,
            donationProcessedEvent.DonorId);

        var donation = await context.Donations
            .FirstOrDefaultAsync(d => d.Id == donationProcessedEvent.DonationId, cancellationToken);

        if (donation is null)
        {
            logger.LogWarning(
                "Doação {DonationId} não encontrada para atualização de status.",
                donationProcessedEvent.DonationId);
            return;
        }

        if (donation.Status == donationStatus)
        {
            logger.LogInformation(
                "Evento duplicado ignorado para a doação {DonationId}. Status atual já é {Status}.",
                donation.Id,
                donation.Status);
            return;
        }

        if (donation.Status == DonationStatus.Paid && donationStatus != DonationStatus.Paid)
        {
            logger.LogWarning(
                "Evento antigo ignorado para a doação {DonationId}. A doação já está paga e recebeu o status {ReceivedStatus}.",
                donation.Id,
                donationStatus);
            return;
        }

        var wasPaid = donation.Status == DonationStatus.Paid;
        donation.UpdateStatus(donationStatus, donationProcessedEvent.ProcessedAt);

        if (!wasPaid && donationStatus == DonationStatus.Paid)
        {
            var campaign = await context.Campaigns
                .FirstOrDefaultAsync(c => c.Id == donation.CampaignId, cancellationToken);

            if (campaign is null)
            {
                throw new InvalidOperationException(
                    $"Campanha {donation.CampaignId} não encontrada para atualizar a doação {donation.Id}.");
            }

            campaign.AddDonation(donation.Amount);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static DonationStatus ParseStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            throw new InvalidOperationException("O status da doação não foi informado.");
        }

        return status.Trim().ToLowerInvariant() switch
        {
            "paid" => DonationStatus.Paid,
            "approved" => DonationStatus.Paid,
            "pending" => DonationStatus.Pending,
            "rejected" => DonationStatus.Rejected,
            _ => throw new InvalidOperationException($"Status de doação desconhecido: {status}.")
        };
    }
}