using Microsoft.AspNetCore.Mvc;
using SolidarityConnection.Application.Features.Auth.Commands.DonorRegistration;
using SolidarityConnection.Application.Features.Auth.Commands.DonorRegistration.Inputs;

namespace SolidarityConnection.Presentation.Controller;

[ApiController]
[Route("api/[controller]")]
public class DonorController : ControllerBase
{
    private readonly IAddOrUpdateDonorCommandHandler _donorRegistrationHandler;

    public DonorController(IAddOrUpdateDonorCommandHandler registerDoadorHandler)
    {
        _donorRegistrationHandler = registerDoadorHandler;
    }

    [HttpPost("Registration")]
    public async Task<IActionResult> Register(
    [FromBody] AddOrUpdateDonorInput input,
    CancellationToken cancellationToken)
    {
        var command = input.MapToCommand();
        var result = await _donorRegistrationHandler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }
}
