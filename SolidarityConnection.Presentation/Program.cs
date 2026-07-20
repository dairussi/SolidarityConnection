using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Prometheus;
using SolidarityConnection.Infrastructure.DI;
using SolidarityConnection.Infrastructure.Persistence;
using SolidarityConnection.Infrastructure.Persistence.Bootstrap;
using SolidarityConnection.Infrastructure.Persistence.Seeds;
using Prometheus.DotNetRuntime;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
    // JWT authentication
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
// .NET runtime metrics (GC, threads, memory)
DotNetRuntimeStatsBuilder.Customize().StartCollecting();

const string serviceName = "SolidarityConnection.API";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource("SolidarityConnection.RabbitMq")
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(
                    builder.Configuration["Jaeger:OtlpEndpoint"] ?? "http://localhost:4317");
            });
    });

var app = builder.Build();
app.UseDefaultFiles();
app.UseDefaultFiles(new DefaultFilesOptions
{
    RequestPath = "/transparencia"
});
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Prometheus middleware must run before MapControllers
app.UseHttpMetrics();

app.MapControllers();

// Endpoint /metrics para o Prometheus fazer scrape
app.MapMetrics();

// Endpoint /health
app.MapHealthChecks("/health");
app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    await context.Database.EnsureCreatedAsync();
    await DonationSchemaBootstrapper.EnsureDonationsTableAsync(context);
    await AdminUserSeed.SeedAsync(context, configuration);
}

app.Run();