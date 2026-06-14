namespace SolidarityConnection.Domain.Donation.Models;
public class Donation
{
    private Donation() { }

    private Donation(Guid campaignId, int donorId, decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException(
                "O valor da doação deve ser maior que zero.");

        Id = Guid.NewGuid();
        CampaignId = campaignId;
        DonorId = donorId;
        Amount = amount;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid CampaignId { get; private set; }
    public int DonorId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Donation Create(Guid campaignId, int donorId, decimal amount)
        => new(campaignId, donorId, amount);
}
