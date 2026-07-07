using CollectionHub.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CollectionHub.Functions.Functions
{
    public class GameFunctions
    {
        private readonly ILogger<GameFunctions> _logger;

        public GameFunctions(ILogger<GameFunctions> logger)
        {
            _logger = logger;
        }

        [Function("GetGames")]
        public IActionResult GetGames([HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetGames")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            List<GameDto> _games =
                [
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Persona 5 Royal",
                    Status = CollectionStatus.Completed,
                    Rating = 10,
                    Notes = "One of my all-time favorites."
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Legend of Zelda: Tears of the Kingdom",
                    Status = CollectionStatus.Completed,
                    Rating = 10,
                    Notes = "Fantastic exploration and gameplay."
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "NieR: Automata",
                    Status = CollectionStatus.Completed,
                    Rating = 10,
                    Notes = "Amazing story and soundtrack."
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Clair Obscur: Expedition 33",
                    Status = CollectionStatus.Playing,
                    Rating = 10,
                    Notes = "Absolutely amazing so far."
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Hollow Knight",
                    Status = CollectionStatus.Backlog,
                    Rating = null,
                    Notes = "Need to finally start it."
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Metaphor: ReFantazio",
                    Status = CollectionStatus.Backlog,
                    Rating = null,
                    Notes = "Looking forward to playing it."
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Elden Ring",
                    Status = CollectionStatus.Backlog,
                    Rating = null,
                    Notes = "Still waiting for the right moment."
                }
            ];

            return new OkObjectResult(_games);
        }
    }
}
