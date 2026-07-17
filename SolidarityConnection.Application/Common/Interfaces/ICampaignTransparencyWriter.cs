namespace SolidarityConnection.Application.Common.Interfaces;

public interface ICampaignTransparencyWriter
{
    Task UpsertCampaignAsync(Guid campaignId, string title, decimal targetAmount, string status, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid campaignId, string status, CancellationToken cancellationToken);
    Task RegisterDonationAsync(Guid campaignId, decimal amount, DateTime processedAt, CancellationToken cancellationToken);
}