using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CollectionHub.Functions.Services.Anime;
using CollectionHub.Functions.Services.Cosmos;
using CollectionHub.Functions.Services.Igdb;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMediaService, CosmosDbMediaService>();
builder.Services.AddHttpClient<IGdbService>();
builder.Services.AddHttpClient<JikanService>();
builder.Services.AddSingleton<SecretClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var keyVaultUri = configuration["AzureKeyVaultUri"]
        ?? throw new InvalidOperationException("AzureKeyVaultUri is missing.");

    return new SecretClient(
        new Uri(keyVaultUri),
        new DefaultAzureCredential());
});
builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
