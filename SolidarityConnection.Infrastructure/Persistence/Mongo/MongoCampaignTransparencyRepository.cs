using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Outputs;
using SolidarityConnection.Infrastructure.Options;

namespace SolidarityConnection.Infrastructure.Persistence.Mongo;

public sealed class MongoCampaignTransparencyRepository : ICampaignTransparencyReader, ICampaignTransparencyWriter
{
    private const int MaxRecentDonations = 10;
    private readonly IMongoCollection<CampaignTransparencyDocument> _collection;

    public MongoCampaignTransparencyRepository(IOptions<MongoOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        _collection = database.GetCollection<CampaignTransparencyDocument>(options.Value.CollectionName);
    }

    public async Task UpsertCampaignAsync(
        Guid campaignId, string title, decimal targetAmount, string status, CancellationToken cancellationToken)
    {
        var update = Builders<CampaignTransparencyDocument>.Update
            .Set(d => d.CampaignId, campaignId)
            .Set(d => d.Title, title)
            .Set(d => d.TargetAmount, targetAmount)
            .Set(d => d.Status, status)
            .SetOnInsert(d => d.AmountRaised, 0m)
            .SetOnInsert(d => d.DonorsCount, 0)
            .SetOnInsert(d => d.LastDonations, new List<RecentDonationDocument>());

        await _collection.UpdateOneAsync(
            d => d.CampaignId == campaignId,
            update,
            new UpdateOptions { IsUpsert = true },
            cancellationToken);
    }

    public async Task UpdateStatusAsync(Guid campaignId, string status, CancellationToken cancellationToken)
    {
        var update = Builders<CampaignTransparencyDocument>.Update.Set(d => d.Status, status);
        await _collection.UpdateOneAsync(d => d.CampaignId == campaignId, update, cancellationToken: cancellationToken);
    }

    public async Task RegisterDonationAsync(
        Guid campaignId, decimal amount, DateTime processedAt, CancellationToken cancellationToken)
    {
        var recentDonation = new RecentDonationDocument { Amount = amount, ProcessedAt = processedAt };

        var update = Builders<CampaignTransparencyDocument>.Update
            .Inc(d => d.AmountRaised, amount)
            .Inc(d => d.DonorsCount, 1)
            .PushEach(d => d.LastDonations, new[] { recentDonation }, slice: -MaxRecentDonations);

        await _collection.UpdateOneAsync(d => d.CampaignId == campaignId, update, cancellationToken: cancellationToken);
    }

    public async Task SyncCampaignSnapshotAsync(
        Guid campaignId,
        string title,
        decimal targetAmount,
        string status,
        decimal amountRaised,
        int donorsCount,
        IReadOnlyCollection<RecentDonationDocument> lastDonations,
        CancellationToken cancellationToken)
    {
        var normalizedDonations = lastDonations
            .OrderByDescending(donation => donation.ProcessedAt)
            .Take(MaxRecentDonations)
            .ToList();

        var update = Builders<CampaignTransparencyDocument>.Update
            .Set(d => d.CampaignId, campaignId)
            .Set(d => d.Title, title)
            .Set(d => d.TargetAmount, targetAmount)
            .Set(d => d.Status, status)
            .Set(d => d.AmountRaised, amountRaised)
            .Set(d => d.DonorsCount, donorsCount)
            .Set(d => d.LastDonations, normalizedDonations);

        await _collection.UpdateOneAsync(
            d => d.CampaignId == campaignId,
            update,
            new UpdateOptions { IsUpsert = true },
            cancellationToken);
    }

    public async Task<(IReadOnlyList<TransparencyCampaignOutput> Items, int TotalCount)> GetActivePagedAsync(
        int page, int pageSize, CancellationToken cancellationToken)
    {
        var filter = Builders<CampaignTransparencyDocument>.Filter.Eq(d => d.Status, "Active");

        var totalCount = (int)await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var documents = await _collection.Find(filter)
            .SortByDescending(d => d.AmountRaised)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        var items = documents.Select(d => new TransparencyCampaignOutput(
            d.CampaignId,
            d.Title,
            d.TargetAmount,
            d.AmountRaised,
            d.DonorsCount,
            d.LastDonations.Select(x => new RecentDonationOutput(x.Amount, x.ProcessedAt)).ToList()
        )).ToList();

        return (items, totalCount);
    }
}