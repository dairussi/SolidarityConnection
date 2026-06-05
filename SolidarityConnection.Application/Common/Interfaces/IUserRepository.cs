using SolidarityConnection.Domain.Users.Models;

namespace SolidarityConnection.Application.Common.Interfaces;


public interface IUserRepository
{
    Task<Donor?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task AddAsync(Donor user, CancellationToken cancellationToken);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
}
