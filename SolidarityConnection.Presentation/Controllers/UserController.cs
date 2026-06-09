using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolidarityConnection.Application.Features.Users.Commands.AddUser;
using SolidarityConnection.Application.Features.Users.Commands.AddUser.Inputs;
using SolidarityConnection.Application.Features.Users.Queries.GetUsersPaged;
using SolidarityConnection.Application.Features.Users.Queries.GetUsersPaged.Inputs;
using SolidarityConnection.Domain.Common.Enums;

namespace SolidarityConnection.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(
        IAddOrUpdateUserCommandHandler addOrUpdateUserCommandHandler,
        IGetUsersPagedQueryHandler getUsersPagedQueryHandler) : ControllerBase
{
    [HttpPost("Registration")]
    public async Task<IActionResult> Register(
        [FromBody] AddOrUpdateUserInput input,
        CancellationToken cancellationToken)
    {
        input.Role = (int)EUserRole.Doador;
        var command = input.MapToCommand();
        var result = await addOrUpdateUserCommandHandler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [Authorize]
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
}
