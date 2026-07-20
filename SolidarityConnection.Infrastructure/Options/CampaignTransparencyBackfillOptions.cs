namespace SolidarityConnection.Infrastructure.Options;

public sealed class CampaignTransparencyBackfillOptions
{
    public int IntervalInMinutes { get; set; } = 30;
}