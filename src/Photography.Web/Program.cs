using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using HealthChecks.NpgSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Photography.Application;
using Photography.Application.Auth;
using Photography.Infrastructure;
using Photography.Infrastructure.Persistence;
using Photography.Web.Auth;
using Photography.Web.Middleware;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// --- Logging ---------------------------------------------------------------
builder.Host.UseSerilog((ctx, logger) => logger
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// --- Application + Infrastructure ------------------------------------------
builder.Services.AddPhotographyApplication();
builder.Services.AddPhotographyInfrastructure(builder.Configuration);

// JWT options + token service (lives in Web because the signing key is bound here).
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddSingleton<ITokenService, JwtTokenService>();

// Contact-form options (notification recipient). Bound here so the Application
// layer remains free of a direct IConfiguration dependency.
builder.Services.Configure<Photography.Application.Contact.Commands.ContactOptions>(
    builder.Configuration.GetSection(Photography.Application.Contact.Commands.ContactOptions.SectionName));

// --- API + OpenAPI ---------------------------------------------------------
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(opts => opts.SuppressMapClientErrors = false);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// .NET 10 native OpenAPI document, augmented with the bearer-token security scheme.
builder.Services.AddOpenApi("v1", opts => opts.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
builder.Services.AddEndpointsApiExplorer();

// --- Health checks ---------------------------------------------------------
var healthChecks = builder.Services.AddHealthChecks();
var connectionString = builder.Configuration.GetConnectionString("Default");
if (!string.IsNullOrEmpty(connectionString))
    healthChecks.AddNpgSql(connectionString, name: "postgres", tags: ["db", "ready"]);
healthChecks.AddCheck<StorageHealthCheck>("storage", tags: ["storage", "ready"]);

// --- CORS ------------------------------------------------------------------
const string corsPolicyName = "PhotographyClient";
builder.Services.AddCors(opts =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    opts.AddPolicy(corsPolicyName, p => p
        .WithOrigins(origins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

// --- Rate limiting ---------------------------------------------------------
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("auth", o =>
    {
        o.Window = TimeSpan.FromMinutes(1);
        o.PermitLimit = 10;
        o.QueueLimit = 0;
    });
    options.AddFixedWindowLimiter("general", o =>
    {
        o.Window = TimeSpan.FromMinutes(1);
        o.PermitLimit = 120;
        o.QueueLimit = 0;
    });
    options.AddFixedWindowLimiter("contact", o =>
    {
        o.Window = TimeSpan.FromMinutes(1);
        o.PermitLimit = 20;
        o.QueueLimit = 0;
    });
});

// --- Auth ------------------------------------------------------------------
var jwtKey = builder.Configuration["Jwt:Key"] ?? string.Empty;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = string.IsNullOrEmpty(jwtKey)
                ? new SymmetricSecurityKey(Encoding.UTF8.GetBytes(new string('0', 64)))
                : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = !string.IsNullOrEmpty(builder.Configuration["Jwt:Issuer"]),
            ValidateAudience = !string.IsNullOrEmpty(builder.Configuration["Jwt:Audience"]),
            ValidateIssuerSigningKey = !string.IsNullOrEmpty(jwtKey),
            ValidateLifetime = true,
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role,
        };
    });

builder.Services.AddAuthorization(opts =>
    opts.AddPolicy("AdminOnly", p => p.RequireAuthenticatedUser().RequireRole("Admin")));

var app = builder.Build();

// --- Pipeline --------------------------------------------------------------
app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();           // serves /openapi/v1.json
    app.MapScalarApiReference(); // serves /scalar/v1 — the modern dev UI
}

app.UseCors(corsPolicyName);
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
});

// Apply EF Core migrations on startup outside the test host (the integration tests use
// the InMemory provider which doesn't support relational migrations).
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsRelational())
        await db.Database.MigrateAsync();
    await Photography.Web.StartupSeeder.SeedAsync(app.Services);
}

app.Run();

public partial class Program { }
