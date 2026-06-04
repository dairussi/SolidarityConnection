using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolidarityConnection.Application.Features.Auth.Commands.DonorRegistration;
using SolidarityConnection.Application.Features.Auth.Commands.DonorRegistration.Inputs;

namespace SolidarityConnection.Presentation.Controller;

[ApiController]
[Route("api/[controller]")]
public class DonorController : ControllerBase
{
    private readonly IDonorRegistrationCommandHandler _donorRegistrationHandler;

    public DonorController(IDonorRegistrationCommandHandler registerDoadorHandler)
    {
        _donorRegistrationHandler = registerDoadorHandler;
    }

    [HttpPost("Registration")]
    public async Task<IActionResult> RegisterDoador(
    [FromBody] DonorRegistrationInput input,
    CancellationToken cancellationToken)
    {
        var command = new DonorRegistrationCommand(input.Name, input.Email, input.Cpf, input.Password);
        var result = await _donorRegistrationHandler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }
}
