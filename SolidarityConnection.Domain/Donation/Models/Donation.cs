using SolidarityConnection.Domain.Donation.Enums;

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
        Status = DonationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid CampaignId { get; private set; }
    public int DonorId { get; private set; }
    public decimal Amount { get; private set; }
    public DonationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    public static Donation Create(Guid campaignId, int donorId, decimal amount)
        => new(campaignId, donorId, amount);

    public void UpdateStatus(DonationStatus status, DateTime processedAt)
    {
        if (Status == DonationStatus.Paid && status != DonationStatus.Paid)
        {
            throw new InvalidOperationException(
                "Uma doação paga não pode voltar para outro status.");
        }

        Status = status;
        ProcessedAt = processedAt;
    }
}
