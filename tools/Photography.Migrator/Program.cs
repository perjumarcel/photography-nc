using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photography.Application.Storage;
using Photography.Core.Albums;
using Photography.Core.Categories;
using Photography.Infrastructure;
using Photography.Infrastructure.Persistence;
using Photography.Migrator;

// Subcommands:
//   migrate-db       — copy legacy SQL Server data into PostgreSQL (preserves IDs).
//   migrate-images   — scan legacy folder, build manifest, optionally upload to R2.
//   verify           — run reconciliation reports.
//
// All commands accept --dry-run to skip side effects.

var rootArgs = args;
if (rootArgs.Length == 0)
{
    Console.WriteLine(Usage());
    return 1;
}

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables(prefix: "PHOTOGRAPHY_")
    .AddCommandLine(rootArgs)
    .Build();

var services = new ServiceCollection();
services.AddLogging(b => b.AddSimpleConsole(o => { o.SingleLine = true; }));
services.AddPhotographyInfrastructure(configuration);
await using var sp = services.BuildServiceProvider();
var logger = sp.GetRequiredService<ILogger<Program>>();

var dryRun = rootArgs.Contains("--dry-run");
var command = rootArgs[0].ToLowerInvariant();

return command switch
{
    "migrate-db" => await DbMigrator.RunAsync(sp, configuration, dryRun, logger),
    "migrate-images" => await ImageMigrator.RunAsync(sp, configuration, dryRun, logger),
    "verify" => await Verifier.RunAsync(sp, logger),
    _ => Help(),
};

static int Help() { Console.WriteLine(Usage()); return 0; }
static string Usage() =>
    "Photography.Migrator <migrate-db|migrate-images|verify> [--dry-run]\n" +
    "Environment / config:\n" +
    "  ConnectionStrings:Default          (PostgreSQL target)\n" +
    "  Migration:LegacyConnectionString   (legacy SQL Server source)\n" +
    "  Migration:LegacyImagesRoot         (e.g. /var/www/photography/Images/Albums/Images)\n" +
    "  Storage:Provider                   Local | R2\n";

public partial class Program { }
