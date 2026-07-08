using CollectionHub.Functions.Services;
using CollectionHub.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace CollectionHub.Functions.Functions
{
    public class GameFunctions
    {
        private readonly ILogger<GameFunctions> _logger;
        private readonly IGameService _gameService;

        public GameFunctions(ILogger<GameFunctions> logger, IGameService gameService)
        {
            _logger = logger;
            _gameService = gameService;
        }

        [Function("GetGames")]
        public IActionResult GetGames([HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetGames")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(_gameService.GetAll());
        }

        [Function("GetGame")]
        public IActionResult GetGame([HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetGame/{gameId}")] HttpRequest req, Guid gameId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            // Here you would typically retrieve the game from a database or in-memory collection using the gameId.
            // For this example, we'll just create a dummy game and return it.
            return new OkObjectResult(_gameService.GetById(gameId));
        }

        [Function("PostGames")]
        public async Task<IActionResult> PostGame([HttpTrigger(AuthorizationLevel.Function, "post", Route = "PostGame")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            GameDto newGame = await req.ReadFromJsonAsync<GameDto>();
            // Here you would typically save the new game to a database or in-memory collection.
            // For this example, we'll just log the new game and return it.
            if(newGame != null)
            {
                _gameService.Add(newGame);
                _logger.LogInformation($"New game added: {newGame.Title}");
            }

            return new OkObjectResult(newGame);
        }

        [Function("UpdateGames")]
        public async Task<IActionResult> UpdateGame([HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateGame")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            GameDto updatedGame = await req.ReadFromJsonAsync<GameDto>();
            // Here you would typically update the game in a database or in-memory collection.
            // For this example, we'll just log the updated game and return it.
            if(updatedGame != null)
            {
                _gameService.Update(updatedGame);
                _logger.LogInformation($"Game updated: {updatedGame.Title}");
            }
            _gameService.Update(updatedGame);
            _logger.LogInformation($"Game updated: {updatedGame.Title}");
            return new OkObjectResult(updatedGame);
        }

        [Function("DeleteGames")]
        public async Task<IActionResult> DeleteGame([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteGame/{gameId}")] HttpRequest req, Guid gameId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            // Here you would typically delete the game from a database or in-memory collection.
            // For this example, we'll just log the deleted game and return a success message.

            _gameService.Delete(gameId);
            _logger.LogInformation($"Game deleted: {gameId}");
            
            return new OkObjectResult($"Game '{gameId}' deleted successfully.");
        }
    }
}
