using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Auth.Queries.Login;
using SolidarityConnection.Application.Features.Campaigns.Commands.CreateCampaign;
using SolidarityConnection.Application.Features.Campaigns.Commands.DeleteCampaign;
using SolidarityConnection.Application.Features.Campaigns.Commands.UpdateCampaignStatus;
using SolidarityConnection.Application.Features.Campaigns.Queries.GetActiveCampaignsPaged;
using SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaignById;
using SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaigns;
using SolidarityConnection.Application.Features.Campaigns.Queries.GetCampaignsPaged;
using SolidarityConnection.Application.Features.Donations.Commands.CreateDonation;
using SolidarityConnection.Application.Features.Donations.Queries.GetMyTotalsByCampaign;
using SolidarityConnection.Application.Features.Users.Commands.AddUser;
using SolidarityConnection.Application.Features.Users.Commands.ToggleUserRole;
using SolidarityConnection.Application.Features.Users.Queries.GetUserById;
using SolidarityConnection.Application.Features.Users.Queries.GetUsersPaged;
using SolidarityConnection.Infrastructure.Adapters.Events.Consumers;
using SolidarityConnection.Infrastructure.Authentication;
using SolidarityConnection.Infrastructure.HostedServices;
using SolidarityConnection.Infrastructure.Messaging;
using SolidarityConnection.Infrastructure.Options;
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
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var userContext = serviceProvider.GetService<IUserContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            options.UseSqlServer(connectionString);
            options.AddInterceptors(new AuditInterceptor(userContext));
        }, ServiceLifetime.Scoped);

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.Configure<PendingDonationReprocessingOptions>(
            configuration.GetSection("BackgroundJobs:PendingDonationReprocessing"));

        var pendingDonationReprocessingOptions =
            configuration
                .GetSection("BackgroundJobs:PendingDonationReprocessing")
                .Get<PendingDonationReprocessingOptions>()
            ?? new PendingDonationReprocessingOptions();

        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<IDonationRepository, DonationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddHttpContextAccessor();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IUserContext, UserContext>();

        services.AddScoped<ILoginQueryHandler, LoginQueryHandler>();
        services.AddScoped<IAddOrUpdateUserCommandHandler, AddOrUpdateUserCommandHandler>();
        services.AddScoped<IToggleUserRoleCommandHandler, ToggleUserRoleCommandHandler>();
        services.AddScoped<IGetUserByIdQueryHandler, GetUserByIdQueryHandler>();
        services.AddScoped<IGetUsersPagedQueryHandler, GetUsersPagedQueryHandler>();
        services.AddScoped<ICreateCampaignCommandHandler, CreateCampaignCommandHandler>();
        services.AddScoped<IDeleteCampaignCommandHandler, DeleteCampaignCommandHandler>();
        services.AddScoped<IUpdateCampaignStatusCommandHandler, UpdateCampaignStatusCommandHandler>();
        services.AddScoped<IGetCampaignByIdQueryHandler, GetCampaignByIdQueryHandler>();
        services.AddScoped<IGetCampaignsQueryHandler, GetCampaignsQueryHandler>();
        services.AddScoped<IGetCampaignsPagedQueryHandler, GetCampaignsPagedQueryHandler>();
        services.AddScoped<IGetActiveCampaignsPagedQueryHandler, GetActiveCampaignsPagedQueryHandler>();
        services.AddScoped<ICreateDonationCommandHandler, CreateDonationCommandHandler>();
        services.AddScoped<IGetMyTotalsByCampaignQueryHandler, GetMyTotalsByCampaignQueryHandler>();
        services.AddScoped<IDonationPaymentDispatcher, DonationPaymentDispatcher>();
        services.AddScoped<DonationProcessedConsumer>();
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
        services.AddHostedService<RabbitMqDonationProcessedConsumerHostedService>();
        services.AddQuartz(quartz =>
        {
            var jobKey = new JobKey(nameof(PendingDonationReprocessingJob));

            quartz.AddJob<PendingDonationReprocessingJob>(options => options.WithIdentity(jobKey));
            quartz.AddTrigger(options => options
                .ForJob(jobKey)
                .WithIdentity($"{nameof(PendingDonationReprocessingJob)}-trigger")
                .StartNow()
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromMinutes(Math.Max(1, pendingDonationReprocessingOptions.IntervalInMinutes)))
                    .RepeatForever()));
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

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
                policy.RequireRole("ONGManager"));
        });

        services.AddHealthChecks()
            .AddSqlServer(
                connectionString: configuration.GetConnectionString("DefaultConnection")!,
                name: "sqlserver",
                tags: new[] { "db", "sql" })
            .AddRabbitMQ(
                rabbitConnectionString: $"amqp://{configuration["RabbitMQ:Username"]}:{configuration["RabbitMQ:Password"]}@{configuration["RabbitMQ:Host"]}:{configuration["RabbitMQ:Port"]}/",
                name: "rabbitmq",
                tags: new[] { "messaging" });

        return services;
    }
}