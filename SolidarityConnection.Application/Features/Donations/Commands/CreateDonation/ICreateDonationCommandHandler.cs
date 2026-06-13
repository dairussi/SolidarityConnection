using SolidarityConnection.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidarityConnection.Application.Features.Donations.Commands.CreateDonation;
public interface ICreateDonationCommandHandler
{
    Task<ResultData<Guid>> Handle(CreateDonationCommand command,CancellationToken cancellationToken);
}
