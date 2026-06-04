using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Auth.Commands.DonorRegistration;
using SolidarityConnection.Application.Features.Auth.Commands.ManagerRegistration;
using SolidarityConnection.Application.Features.Auth.Queries.Login;
using SolidarityConnection.Infrastructure.Persistence;
using SolidarityConnection.Infrastructure.Repositories;
using SolidarityConnection.Infrastructure.Services;

namespace SolidarityConnection.Infrastructure.DI;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Banco de dados
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        // Repositórios
        services.AddScoped<IUserRepository, UserRepository>();

        // Serviços
        services.AddScoped<ITokenService, JwtTokenService>();

        // Handlers — cada interface mapeada para sua implementação
        services.AddScoped<ILoginQueryHandler, LoginQueryHandler>();
        services.AddScoped<IDonorRegistrationCommandHandler, DonorRegistrationCommandHandler>();
        services.AddScoped<IManagerRegistrationCommandHandler, ManagerRegistrationCommandHandler>();

        // JWT
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:SecretKey"]!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };
        });

        services.AddAuthorization();

        return services;
    }
}