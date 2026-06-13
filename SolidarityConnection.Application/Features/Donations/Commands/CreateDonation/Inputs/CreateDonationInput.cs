namespace SolidarityConnection.Application.Features.Donations.Commands.CreateDonation.Inputs;
public class CreateDonationInput
{
    public Guid CampaignId { get; set; }
    public decimal Amount { get; set; }

    public CreateDonationCommand MapToCommand(int donorId)
        => new(CampaignId, donorId, Amount);
}
