using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Domain.Campaign.Enums;
using SolidarityConnection.Domain.Donation.Models;

namespace SolidarityConnection.Application.Features.Donations.Commands.CreateDonation;

public sealed class CreateDonationCommandHandler(
    ICampaignRepository campaignRepository,
    IDonationRepository donationRepository,
    IDonationPaymentDispatcher donationPaymentDispatcher) : ICreateDonationCommandHandler
{
    public async Task<ResultData<Guid>> Handle(
        CreateDonationCommand command,
        CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetByIdAsync(command.CampaignId, cancellationToken);

        if (campaign is null)
        {
            return ResultData<Guid>.Error("Campanha não encontrada.");
        }

        if (campaign.Status != CampaignStatus.Active)
        {
            return ResultData<Guid>.Error(
                "Não é possível realizar doações para campanhas encerradas ou pausadas.");
        }

        var donation = Donation.Create(
            command.CampaignId,
            command.DonorId,
            command.Amount);

        await donationRepository.AddAsync(donation, cancellationToken);
        await donationPaymentDispatcher.DispatchAsync(donation, cancellationToken);

        return ResultData<Guid>.Success(donation.Id);
    }
}
