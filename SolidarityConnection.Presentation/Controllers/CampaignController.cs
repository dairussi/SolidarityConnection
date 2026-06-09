using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Commands.CreateCampaign;
using SolidarityConnection.Application.Features.Campaigns.Commands.CreateCampaign.Inputs;
using SolidarityConnection.Application.Features.Campaigns.Commands.DeleteCampaign;
using SolidarityConnection.Application.Features.Campaigns.Commands.UpdateCampaignStatus;
using SolidarityConnection.Application.Features.Campaigns.Commands.UpdateCampaignStatus.Inputs;
using SolidarityConnection.Application.Features.Campaigns.Queries.GetActiveCampaignsPaged;
using SolidarityConnection.Application.Features.Campaigns.Queries.GetActiveCampaignsPaged.Inputs;
using SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaignById;
using SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaignsPaged;
using SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaignsPaged.Inputs;
using SolidarityConnection.Domain.Common.Enums;

namespace SolidarityConnection.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController(
    ICreateCampaignCommandHandler createCampaignCommandHandler,
    IDeleteCampaignCommandHandler deleteCampaignCommandHandler,
    IUpdateCampaignStatusCommandHandler updateCampaignStatusCommandHandler,
    IGetCampaignByIdQueryHandler getCampaignByIdQueryHandler,
    IGetCampaignsPagedQueryHandler getCampaignsPagedQueryHandler,
    IGetActiveCampaignsPagedQueryHandler getActiveCampaignsPagedQueryHandler,
    IUserContext userContext) : ControllerBase
{

    [Authorize(Roles = nameof(EUserRole.GestorONG))]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCampaignInput input,
        CancellationToken cancellationToken)
    {
        var command = input.MapToCommand(userContext.GetCurrentUserId());
        var result = await createCampaignCommandHandler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await getCampaignByIdQueryHandler.Handle(new GetCampaignByIdQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] GetCampaignsPagedInput input,
        CancellationToken cancellationToken)
    {
        var result = await getCampaignsPagedQueryHandler.Handle(input.MapToQuery(), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [AllowAnonymous]
    [HttpGet("active")]
    public async Task<IActionResult> GetActive(
        [FromQuery] GetActiveCampaignsPagedInput input,
        CancellationToken cancellationToken)
    {
        var result = await getActiveCampaignsPagedQueryHandler.Handle(input.MapToQuery(), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [Authorize]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateCampaignStatusInput input,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCampaignStatusCommand(id, input.Status);
        var result = await updateCampaignStatusCommandHandler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteCampaignCommandHandler.Handle(new DeleteCampaignCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { message = result.ErrorMessage });

        return NoContent();
    }
}
