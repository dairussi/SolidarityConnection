using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Auth.Queries.Login;
using SolidarityConnection.Application.Features.Users.Commands.AddUser;
using SolidarityConnection.Infrastructure.Authentication;
using SolidarityConnection.Infrastructure.Persistence;
using SolidarityConnection.Infrastructure.Persistence.Interceptors;
using SolidarityConnection.Infrastructure.Repositories;
using SolidarityConnection.Infrastructure.Services;
using System.Text;

namespace SolidarityConnection.Infrastructure.DI;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Banco de dados
        //services.AddDbContext<AppDbContext>(options =>
        //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var userContext = serviceProvider.GetService<IUserContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString);
            options.AddInterceptors(new AuditInterceptor(userContext));
        }, ServiceLifetime.Scoped);

        // Repositórios
        services.AddScoped<IUserRepository, UserRepository>();

        // Serviços
        services.AddHttpContextAccessor();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IUserContext, UserContext>();

        // Handlers — cada interface mapeada para sua implementação
        services.AddScoped<ILoginQueryHandler, LoginQueryHandler>();
        services.AddScoped<IAddOrUpdateUserCommandHandler, AddOrUpdateUserCommandHandler>();
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

            options.Events = new JwtBearerEvents
            {
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Usuário não possui permissão para acessar este recurso."
                    });
                },

                OnChallenge = async context =>
                {
                    context.HandleResponse();

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Usuário não autenticado."
                    });
                }
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("OnlyManagers", policy =>
                policy.RequireRole("GestorONG"));
        });

        return services;
    }
}