using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SolidarityConnection.Infrastructure.DI;
using SolidarityConnection.Infrastructure.Persistence;
using SolidarityConnection.Infrastructure.Persistence.Bootstrap;
using SolidarityConnection.Infrastructure.Persistence.Seeds;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API da SolidarityConnection",
        Version = "v1"
    });

    // Autenticação JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Informe o token JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    await context.Database.EnsureCreatedAsync();
    await DonationSchemaBootstrapper.EnsureDonationsTableAsync(context);
    await AdminUserSeed.SeedAsync(context, configuration);
}

app.Run();
