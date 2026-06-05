using Microsoft.AspNetCore.Mvc;
using SolidarityConnection.Application.Features.Users.Commands.AddUser;
using SolidarityConnection.Application.Features.Users.Commands.AddUser.Inputs;
using SolidarityConnection.Domain.Common.Enums;

namespace SolidarityConnection.Presentation.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IAddOrUpdateUserCommandHandler _addOrUpdateUserCommandHandler;

    public UserController(IAddOrUpdateUserCommandHandler addOrUpdateUserCommandHandler)
    {
        _addOrUpdateUserCommandHandler = addOrUpdateUserCommandHandler;
    }

    [HttpPost("Registration")]
    public async Task<IActionResult> Register(
    [FromBody] AddOrUpdateUserInput input,
    CancellationToken cancellationToken)
    {
        input.Role = (int)EUserRole.Doador;
        var command = input.MapToCommand();
        var result = await _addOrUpdateUserCommandHandler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }
}
