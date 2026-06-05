using Microsoft.AspNetCore.Mvc;
using SolidarityConnection.Application.Features.Auth.Queries.Login;
using SolidarityConnection.Application.Features.Auth.Queries.Login.Inputs;

namespace SolidarityConnection.Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginQueryHandler _loginHandler;

        public AuthController(ILoginQueryHandler loginHandler)
        {
            _loginHandler = loginHandler;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginQueryInput input,
            CancellationToken cancellationToken)
        {
            var result = await _loginHandler.Handle(input.MapToQuery(), cancellationToken);

            if (!result.IsSuccess)
                return Unauthorized(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }
    }
}
