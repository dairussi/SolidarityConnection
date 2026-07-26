using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Donations.Commands.CreateDonation;
using SolidarityConnection.Application.Features.Donations.Commands.CreateDonation.Inputs;
using SolidarityConnection.Application.Features.Donations.Queries.GetMyTotalsByCampaign;
using SolidarityConnection.Domain.Common.Enums;

namespace SolidarityConnection.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DonationController(
    ICreateDonationCommandHandler createDonationCommandHandler,
    IGetMyTotalsByCampaignQueryHandler getMyTotalsByCampaignQueryHandler,
    IUserContext userContext) : ControllerBase
{
    [Authorize(Roles = nameof(EUserRole.Doador))]
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

    [Authorize(Roles = nameof(EUserRole.Doador))]
    [HttpGet("MyTotalsByCampaign")]
    public async Task<IActionResult> GetMyTotalsByCampaign(CancellationToken cancellationToken)
    {
        var query = new GetMyTotalsByCampaignQuery(userContext.GetCurrentUserId());
        var result = await getMyTotalsByCampaignQueryHandler.Handle(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }
}