using Microsoft.AspNetCore.Http;
using SolidarityConnection.Application.Common.Interfaces;
using System.Security.Claims;

namespace SolidarityConnection.Infrastructure.Authentication;
public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public int GetCurrentUserId()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User
          .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("Usuário não autenticado");

        return int.Parse(userIdClaim);
    }

}
