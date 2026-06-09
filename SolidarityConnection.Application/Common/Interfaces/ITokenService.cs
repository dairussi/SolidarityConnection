namespace SolidarityConnection.Application.Common.Interfaces;
public interface ITokenService
{
    string GenerateToken(int userId, string role);
}
