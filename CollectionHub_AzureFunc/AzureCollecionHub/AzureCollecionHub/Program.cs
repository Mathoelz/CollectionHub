using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using CollectionHub.Functions.Services.Anime;
using CollectionHub.Functions.Services.Cosmos;
using CollectionHub.Functions.Services.Covers;
using CollectionHub.Functions.Services.Igdb;
using CollectionHub.Functions.Services.Secrets;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMediaService, CosmosDbMediaService>();
builder.Services.AddSingleton<ISecretProvider, KeyVaultSecretProvider>();
builder.Services.AddHttpClient<IGdbService>();
builder.Services.AddHttpClient<JikanService>();
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


// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

host.Run();
