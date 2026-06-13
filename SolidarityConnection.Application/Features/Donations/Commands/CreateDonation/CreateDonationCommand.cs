using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidarityConnection.Application.Features.Donations.Commands.CreateDonation;
public sealed record CreateDonationCommand(
    Guid CampaignId,
    int DonorId,
    decimal Amount);
