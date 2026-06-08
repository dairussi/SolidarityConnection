using Microsoft.AspNetCore.Mvc;
using SolidarityConnection.Application.Features.Auth.Queries.Login;
using SolidarityConnection.Application.Features.Auth.Queries.Login.Inputs;

namespace SolidarityConnection.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ILoginQueryHandler loginHandler) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginQueryInput input,
        CancellationToken cancellationToken)
    {
        var result = await loginHandler.Handle(input.MapToQuery(), cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }
}
