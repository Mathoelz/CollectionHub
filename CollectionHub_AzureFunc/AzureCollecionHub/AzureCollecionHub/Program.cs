using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using CollectionHub.Functions.Services.Anime;
using CollectionHub.Functions.Services.Cosmos;
using CollectionHub.Functions.Services.Covers;
using CollectionHub.Functions.Services.Igdb;
using CollectionHub.Functions.Services.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Azure.Functions.Worker;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(
        builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IMediaService, CosmosDbMediaService>();
builder.Services.AddSingleton<ISecretProvider, KeyVaultSecretProvider>();
builder.Services.AddHttpClient<IGameSearchService, IGdbService>();
builder.Services.AddHttpClient<IAnimeSearchService, AniListService>();
builder.Services.AddHttpClient<ICoverService, CoverService>();
builder.Services.AddSingleton<SecretClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var keyVaultUri = configuration["AzureKeyVaultUri"]
        ?? throw new InvalidOperationException("AzureKeyVaultUri is missing.");

    return new SecretClient(
        new Uri(keyVaultUri),
        new DefaultAzureCredential());
});
builder.Services.AddSingleton<BlobServiceClient>(ServiceProvider =>
{
    var configuration = ServiceProvider.GetRequiredService<IConfiguration>();

    var blobStorageUri = configuration["AzureBlobStorageUri"]
        ?? throw new InvalidOperationException("AzureBlobStorageUri is missing.");

    return new BlobServiceClient(
        new Uri(blobStorageUri),
        new DefaultAzureCredential());
});
builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
builder.Services
     .AddApplicationInsightsTelemetryWorkerService()
     .ConfigureFunctionsApplicationInsights();

builder.Logging.Services.Configure<LoggerFilterOptions>(options =>
{
    var defaultRule = options.Rules.FirstOrDefault(rule =>
        rule.ProviderName ==
        "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");

    if (defaultRule is not null)
    {
        options.Rules.Remove(defaultRule);
    }
});

builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("System", LogLevel.Warning);
builder.Logging.AddFilter("CollectionHub", LogLevel.Information);

var host = builder.Build();

try
{
    var secretProvider = host.Services.GetRequiredService<ISecretProvider>();

    await secretProvider.InitializeAsync();
}
catch(Exception ex)
{
    Console.WriteLine(ex);
}

host.Run();
