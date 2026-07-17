using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SolidarityConnection.Infrastructure.Persistence.Mongo;

[BsonIgnoreExtraElements]
public sealed class CampaignTransparencyDocument
{
    public Guid CampaignId { get; set; }
    public string Title { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal TargetAmount { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal AmountRaised { get; set; }

    public int DonorsCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<RecentDonationDocument> LastDonations { get; set; } = new();
}

public sealed class RecentDonationDocument
{
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Amount { get; set; }
    public DateTime ProcessedAt { get; set; }
}