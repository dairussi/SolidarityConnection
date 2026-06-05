using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SolidarityConnection.Domain.Common.Enums;
using SolidarityConnection.Domain.Users.Models;


namespace SolidarityConnection.Infrastructure.Persistence.Seeds;
public static class AdminUserSeed
{
    public static async Task SeedAsync(AppDbContext context, IConfiguration configuration)
    {
        var email = configuration["AdminSeed:Email"]!;

        if (await context.Users.AnyAsync(u => u.Email == email))
            return;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(configuration["AdminSeed:Password"]!);

        var admin = Donor.Create(
            name: configuration["AdminSeed:Name"]!,
            email: email,
            cpf: string.Empty,
            passwordHash: passwordHash,
            role: UserRole.GestorONG);

        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();
    }
}
