namespace SolidarityConnection.Application.Features.Donations.Outputs;

public sealed record DonationTotalByCampaignOutput(
    Guid CampaignId,
    string CampaignTitle,
    decimal TotalDonated);