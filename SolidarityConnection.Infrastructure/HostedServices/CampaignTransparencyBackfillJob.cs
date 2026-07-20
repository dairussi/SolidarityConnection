using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using SolidarityConnection.Domain.Donation.Enums;
using SolidarityConnection.Infrastructure.Persistence;
using SolidarityConnection.Infrastructure.Persistence.Mongo;

namespace SolidarityConnection.Infrastructure.HostedServices;

[DisallowConcurrentExecution]
public sealed class CampaignTransparencyBackfillJob(
    AppDbContext context,
    MongoCampaignTransparencyRepository transparencyRepository,
    ILogger<CampaignTransparencyBackfillJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext contextExecution)
    {
        var cancellationToken = contextExecution.CancellationToken;
        var startedAtUtc = DateTime.UtcNow;

        logger.LogInformation(
            "Iniciando sincronização de campanhas para o portal da transparência às {StartedAtUtc}.",
            startedAtUtc);

        var campaigns = await context.Campaigns
            .AsNoTracking()
            .Select(campaign => new
            {
                campaign.Id,
                campaign.Title,
                campaign.TargetAmount,
                Status = campaign.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        var paidDonations = await context.Donations
            .AsNoTracking()
            .Where(donation => donation.Status == DonationStatus.Paid)
            .Select(donation => new
            {
                donation.CampaignId,
                donation.Amount,
                donation.ProcessedAt
            })
            .ToListAsync(cancellationToken);

        var donationsByCampaign = paidDonations
            .Where(donation => donation.ProcessedAt.HasValue)
            .GroupBy(donation => donation.CampaignId)
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (var campaign in campaigns)
        {
            cancellationToken.ThrowIfCancellationRequested();

            donationsByCampaign.TryGetValue(campaign.Id, out var campaignDonations);
            campaignDonations ??= [];

            await transparencyRepository.SyncCampaignSnapshotAsync(
                campaign.Id,
                campaign.Title,
                campaign.TargetAmount,
                campaign.Status,
                campaignDonations.Sum(donation => donation.Amount),
                campaignDonations.Count,
                campaignDonations
                    .OrderByDescending(donation => donation.ProcessedAt)
                    .Take(10)
                    .Select(donation => new RecentDonationDocument
                    {
                        Amount = donation.Amount,
                        ProcessedAt = donation.ProcessedAt!.Value
                    })
                    .ToList(),
                cancellationToken);
        }

        logger.LogInformation(
            "Sincronização de campanhas para o portal da transparência finalizada às {FinishedAtUtc}. Campanhas processadas: {CampaignsCount}.",
            DateTime.UtcNow,
            campaigns.Count);
    }
}