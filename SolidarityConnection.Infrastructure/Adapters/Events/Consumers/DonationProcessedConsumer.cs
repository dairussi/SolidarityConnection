using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Domain.Donation.Enums;
using SolidarityConnection.Infrastructure.Messaging.Events;
using SolidarityConnection.Infrastructure.Persistence;

namespace SolidarityConnection.Infrastructure.Adapters.Events.Consumers;

public sealed class DonationProcessedConsumer(
    AppDbContext context,
    ICampaignTransparencyWriter transparencyWriter,
    ILogger<DonationProcessedConsumer> logger)
{
    public async Task ConsumeAsync(
        DonationProcessedEvent donationProcessedEvent,
        CancellationToken cancellationToken)
    {
        var donationStatus = ParseStatus(donationProcessedEvent.Status);

        var donation = await context.Donations
            .FirstOrDefaultAsync(d => d.Id == donationProcessedEvent.DonationId, cancellationToken);

        if (donation is null)
        {
            logger.LogWarning("Doação {DonationId} não encontrada.", donationProcessedEvent.DonationId);
            return;
        }

        if (donation.Status == DonationStatus.Paid)
        {
            logger.LogInformation(
                "Doação {DonationId} já estava confirmada como Paid. Mensagem duplicada/reentregue ignorada com segurança.",
                donation.Id);
            return;
        }

        donation.UpdateStatus(donationStatus, donationProcessedEvent.ProcessedAt);

        SolidarityConnection.Domain.Campaign.Models.Campaign? campaign = null;

        if (donationStatus == DonationStatus.Paid)
        {
            campaign = await context.Campaigns
                .FirstOrDefaultAsync(c => c.Id == donation.CampaignId, cancellationToken);

            if (campaign is null)
                throw new InvalidOperationException($"Campanha {donation.CampaignId} não encontrada.");

            campaign.AddDonation(donation.Amount);
        }

        await context.SaveChangesAsync(cancellationToken);

        if (campaign is not null)
        {
            try
            {
                await transparencyWriter.RegisterDonationAsync(
                    campaign.Id, donation.Amount, donationProcessedEvent.ProcessedAt, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Falha ao atualizar o read model do Mongo para a campanha {CampaignId}. O SQL já foi salvo com sucesso.",
                    campaign.Id);
            }
        }
    }

    private static DonationStatus ParseStatus(string status) =>
        status.Trim().ToLowerInvariant() switch
        {
            "paid" or "approved" => DonationStatus.Paid,
            "pending" => DonationStatus.Pending,
            "rejected" => DonationStatus.Rejected,
            _ => throw new InvalidOperationException($"Status de doação desconhecido: {status}.")
        };
}
