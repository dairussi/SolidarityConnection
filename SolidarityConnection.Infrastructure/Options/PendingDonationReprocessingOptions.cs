namespace SolidarityConnection.Infrastructure.Options;

public sealed class PendingDonationReprocessingOptions
{
    public int IntervalInMinutes { get; set; } = 2;
}
