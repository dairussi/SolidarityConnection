using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Donations.Commands.CreateDonation;
using SolidarityConnection.Application.Features.Donations.Commands.CreateDonation.Inputs;

namespace SolidarityConnection.Presentation.Controllers;
[ApiController]
[Route("api/[controller]")]
public class DonationController(
    ICreateDonationCommandHandler createDonationCommandHandler,
    IUserContext userContext) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDonationInput input,
        CancellationToken cancellationToken)
    {
        var command = input.MapToCommand(userContext.GetCurrentUserId());
        var result = await createDonationCommandHandler
            .Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Accepted(new { donationId = result.Data, message = "Sua doação foi recebida e será processada em breve." });
    }
}
