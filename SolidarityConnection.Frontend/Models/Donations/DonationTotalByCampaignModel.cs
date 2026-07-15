namespace SolidarityConnection.Frontend.Models.Donations;

public sealed class DonationTotalByCampaignModel
{
    public Guid CampaignId { get; set; }

    public string CampaignTitle { get; set; } = string.Empty;

    public decimal TotalDonated { get; set; }
}