using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolidarityConnection.Application.Features.Users.Commands.AddUser;
using SolidarityConnection.Application.Features.Users.Commands.AddUser.Inputs;
using SolidarityConnection.Application.Features.Users.Commands.ToggleUserRole;
using SolidarityConnection.Application.Features.Users.Commands.ToggleUserRole.Inputs;
using SolidarityConnection.Application.Features.Users.Queries.GetUserById;
using SolidarityConnection.Application.Features.Users.Queries.GetUserById.Inputs;
using SolidarityConnection.Application.Features.Users.Queries.GetUsersPaged;
using SolidarityConnection.Application.Features.Users.Queries.GetUsersPaged.Inputs;
using SolidarityConnection.Domain.Common.Enums;

namespace SolidarityConnection.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(
        IAddOrUpdateUserCommandHandler addOrUpdateUserCommandHandler,
        IGetUsersPagedQueryHandler getUsersPagedQueryHandler,
        IGetUserByIdQueryHandler getUserByIdQueryHandler,
        IToggleUserRoleCommandHandler toggleUserRoleCommandHandler) : ControllerBase
{
    [HttpPost("Registration")]
    public async Task<IActionResult> Register(
        [FromBody] AddOrUpdateUserInput input,
        CancellationToken cancellationToken)
    {
        var command = input.MapToCommand();
        var result = await addOrUpdateUserCommandHandler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [Authorize(Roles = nameof(EUserRole.GestorONG))]
    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] GetUsersPagedInput input,
        CancellationToken cancellationToken)
    {
        var result = await getUsersPagedQueryHandler.Handle(input.MapToQuery(), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [Authorize(Roles = nameof(EUserRole.GestorONG))]
    [HttpGet("ById")]
    public async Task<IActionResult> GetById(
        [FromQuery] GetUserByIdInput input,
        CancellationToken cancellationToken)
    {
        var result = await getUserByIdQueryHandler.Handle(input.MapToQuery(), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [Authorize(Roles = nameof(EUserRole.GestorONG))]
    [HttpPatch("Role")]
    public async Task<IActionResult> ToggleRole(
        [FromBody] ToggleUserRoleInput input,
        CancellationToken cancellationToken)
    {
        var result = await toggleUserRoleCommandHandler.Handle(input.MapToCommand(), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }
}
