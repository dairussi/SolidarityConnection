namespace SolidarityConnection.Application.Features.Campaigns.Outputs;

public record TransparencyCampaignOutput
(
    Guid Id,
    string Title,
    decimal TargetAmount,
    decimal AmountRaised,
    int DonorsCount,
    IReadOnlyList<RecentDonationOutput> LastDonations
);

public record RecentDonationOutput(decimal Amount, DateTime ProcessedAt);