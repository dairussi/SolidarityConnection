using Microsoft.EntityFrameworkCore;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Domain.Models;
using SolidarityConnection.Infrastructure.Persistence;

namespace SolidarityConnection.Infrastructure.Repositories;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        => await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        => await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
}
