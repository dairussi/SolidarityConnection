using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidarityConnection.Application.Features.Donations.Commands.CreateDonation;
public record DonationReceivedEvent(
    Guid DonationId,
    Guid CampaignId,
    int DonorId,
    decimal Amount,
    DateTime CreatedAt);
