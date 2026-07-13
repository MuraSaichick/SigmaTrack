using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SigmaTrack.Application.Common;
using SigmaTrack.Application.Features.Issues.CreateIssue;
using SigmaTrack.Application.Interfaces;
using SigmaTrack.Infrastructure;
using SigmaTrack.Infrastructure.Authentication;
using SigmaTrack.Infrastructure.Common;
using SigmaTrack.Infrastructure.Data;
using SigmaTrack.Infrastructure.Repositories;
using SigmaTrack.WebApi.Endpoints;
using SigmaTrack.WebApi.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string \"DefaultConnection\" not found");

builder.Services.AddCors(options =>
{
    options.AddPolicy("NuxtApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection)
.UseSnakeCaseNamingConvention());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IStorageService, LocalStorageService>();

builder.Services.AddInfrastructureRepositories(typeof(IssueRepository).Assembly);

builder.Services.AddApplicationUseCases(typeof(CreateIssueUseCase).Assembly);

var app = builder.Build();
app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Не удалось применить миграции при старте контейнера.");
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseCors("NuxtApp");
    app.Use(async (context, next) =>
    {
        var referer = context.Request.Headers["Referer"].ToString();
        var isFromScalar = referer.Contains("/scalar/") || referer.Contains("/scalar");

        if (isFromScalar && !context.Request.Headers.ContainsKey("Authorization"))
        {
            var testTokem = app.Configuration["Jwt:TestToken"];
            context.Request.Headers["Authorization"] = $"Bearer {testTokem}";
            Console.WriteLine("JWT добавлен в запрос из Scalar");
        }
        await next();
    });
}

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var endpointModules = typeof(Program).Assembly.GetTypes()
    .Where(t => typeof(IEndpointModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
    .Select(Activator.CreateInstance)
    .Cast<IEndpointModule>();
foreach (var module in endpointModules)
{
    module.MapEndpoints(app);
}
app.Run();