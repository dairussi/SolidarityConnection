using Microsoft.EntityFrameworkCore;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Domain.User.Models;
using SolidarityConnection.Infrastructure.Persistence;

namespace SolidarityConnection.Infrastructure.Repositories;
public class UserRepository(AppDbContext context) : IUserRepository
{

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        => await context.Users
        .FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        => await context.Users
        .AnyAsync(u => u.Email.Value == email, cancellationToken);

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = context.Users
            .AsNoTracking()
            .OrderBy(u => u.Name);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
