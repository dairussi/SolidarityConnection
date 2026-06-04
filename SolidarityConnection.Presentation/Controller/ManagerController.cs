using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolidarityConnection.Application.Features.Auth.Commands.ManagerRegistration;
using SolidarityConnection.Application.Features.Auth.Commands.ManagerRegistration.Inputs;

namespace SolidarityConnection.Presentation.Controller;
[ApiController]
[Route("api/[controller]")]
public class ManagerController : ControllerBase
{
    private readonly IManagerRegistrationCommandHandler _managerRegistrationHandler;

    public ManagerController(IManagerRegistrationCommandHandler registerGestorHandler)
    {
        _managerRegistrationHandler = registerGestorHandler;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "GestorONG")]
    [HttpPost("Registration")]
    public async Task<IActionResult> RegisterGestor(
    [FromBody] ManagerRegistrationInput input,
    CancellationToken cancellationToken)
    {
        var command = new ManagerRegistrationCommand(input.Name, input.Email, input.Password);
        var result = await _managerRegistrationHandler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }
}
