using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AzureCollecionHub;

public class HelloCollectionHubFunction
{
    private readonly ILogger<HelloCollectionHubFunction> _logger;

    public HelloCollectionHubFunction(ILogger<HelloCollectionHubFunction> logger)
    {
        _logger = logger;
    }

    [Function("HelloCollectionHubFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string jsonString = JsonSerializer.Serialize(new { message = "Hello, Collection Hub!" });

        return new OkObjectResult(jsonString);
    }
}