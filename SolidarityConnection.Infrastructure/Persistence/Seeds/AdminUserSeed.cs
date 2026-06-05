using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SolidarityConnection.Domain.Common.Enums;
using SolidarityConnection.Domain.User.Models;
using SolidarityConnection.Domain.User.ValueObjects;

namespace SolidarityConnection.Infrastructure.Persistence.Seeds;
public static class AdminUserSeed
{
    public static async Task SeedAsync(AppDbContext context, IConfiguration configuration)
    {
        var emailValue = configuration["AdminSeed:Email"]!;

        if (await context.Users.AnyAsync(u => u.Email.Value == emailValue))
            return;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(configuration["AdminSeed:Password"]!);

        var email = EmailAddress.Create(emailValue);
        var cpf = CpfValidator.Create(configuration["AdminSeed:CPF"]!);

        var admin = User.Create(
            name: configuration["AdminSeed:Name"]!,
            email: email,
            cpf: cpf,
            passwordHash: passwordHash,
            role: EUserRole.GestorONG);

        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();
    }
}
