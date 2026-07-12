using CollectionHub.Functions.Services;
using CollectionHub.Shared.Dtos.Anime;
using CollectionHub.Shared.Dtos.Game;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace CollectionHub.Functions.Functions
{
    public class MediaFunctions
    {
        private readonly ILogger<MediaFunctions> _logger;
        private readonly IMediaService _mediaService;
        private readonly IGdbService _igdbService;
        private readonly JikanService _jikanService;

        public MediaFunctions(ILogger<MediaFunctions> logger, IMediaService mediaService, IGdbService igdbService, JikanService jikanService)
        {
            _logger = logger;
            _mediaService = mediaService;
            _igdbService = igdbService;
            _jikanService = jikanService;
        }

        #region Games

        [Function("GetGames")]
        public async Task<IActionResult> GetGames([HttpTrigger(AuthorizationLevel.Function, "get", Route = "games")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(await _mediaService.GetAllGames());
        }

        [Function("GetGame")]
        public async Task<IActionResult> GetGame([HttpTrigger(AuthorizationLevel.Function, "get", Route = "games/{gameId}")] HttpRequest req, Guid gameId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            // Here you would typically retrieve the game from a database or in-memory collection using the gameId.
            // For this example, we'll just create a dummy game and return it.
            return new OkObjectResult(await _mediaService.GetGameById(gameId));
        }

        [Function("PostGames")]
        public async Task<IActionResult> PostGame([HttpTrigger(AuthorizationLevel.Function, "post", Route = "games")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            GameDto newGame = await req.ReadFromJsonAsync<GameDto>();
            // Here you would typically save the new game to a database or in-memory collection.
            // For this example, we'll just log the new game and return it.
            if(newGame != null)
            {
                await _mediaService.AddGame(newGame);
                _logger.LogInformation($"New game added: {newGame.Title}");
            }

            return new OkObjectResult(newGame);
        }

        [Function("UpdateGames")]
        public async Task<IActionResult> UpdateGame([HttpTrigger(AuthorizationLevel.Function, "put", Route = "games")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            GameDto updatedGame = await req.ReadFromJsonAsync<GameDto>();
            // Here you would typically update the game in a database or in-memory collection.
            // For this example, we'll just log the updated game and return it.
            if(updatedGame != null)
            {
                await _mediaService.UpdateGame(updatedGame);
                _logger.LogInformation($"Game updated: {updatedGame.Title}");
            }
            return new OkObjectResult(updatedGame);
        }

        [Function("DeleteGames")]
        public async Task<IActionResult> DeleteGame([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "games/{gameId}")] HttpRequest req, Guid gameId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            // Here you would typically delete the game from a database or in-memory collection.
            // For this example, we'll just log the deleted game and return a success message.

            await _mediaService.DeleteItem(gameId);
            _logger.LogInformation($"Game deleted: {gameId}");
            
            return new OkObjectResult($"Game '{gameId}' deleted successfully.");
        }

        [Function("SearchGames")]
        public async Task<IActionResult> SearchGame([HttpTrigger(AuthorizationLevel.Function, "get", Route = "games/search/{gameName}")] HttpRequest req, string gameName)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
          
            return new OkObjectResult(await _igdbService.SearchGamesAsync(gameName));
        }

        [Function("SearchCovers")]
        public async Task<IActionResult> SearchCovers([HttpTrigger(AuthorizationLevel.Function, "get", Route = "games/covers/{gameId}")] HttpRequest req, int gameId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(await _igdbService.GetCoverAsync(gameId));
        }

        #endregion

        #region Anime

        [Function("GetAnimes")]
        public async Task<IActionResult> GetAnimes([HttpTrigger(AuthorizationLevel.Function, "get", Route = "animes")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult(await _mediaService.GetAllAnimes());
        }

        [Function("GetAnime")]
        public async Task<IActionResult> GetAnime([HttpTrigger(AuthorizationLevel.Function, "get", Route = "animes/{animeId}")] HttpRequest req, Guid animeId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult(await _mediaService.GetAnimeById(animeId));
        }

        [Function("AddAnime")]
        public async Task<IActionResult> PostAnime([HttpTrigger(AuthorizationLevel.Function, "post", Route = "animes")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            AnimeDto newAnime = await req.ReadFromJsonAsync<AnimeDto>();
            if (newAnime != null)
            {
                await _mediaService.AddAnime(newAnime);
                _logger.LogInformation($"New anime added: {newAnime.Title}");
            }
            return new OkObjectResult(newAnime);
        }

        [Function("UpdateAnime")]
        public async Task<IActionResult> UpdateAnime([HttpTrigger(AuthorizationLevel.Function, "put", Route = "animes")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            AnimeDto updatedAnime = await req.ReadFromJsonAsync<AnimeDto>();
            if (updatedAnime != null)
            {
                await _mediaService.UpdateAnime(updatedAnime);
                _logger.LogInformation($"Anime updated: {updatedAnime.Title}");
            }
            return new OkObjectResult(updatedAnime);
        }

        [Function("DeleteAnime")]
        public async Task<IActionResult> DeleteAnime([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "animes/{animeId}")] HttpRequest req, Guid animeId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            await _mediaService.DeleteItem(animeId);
            _logger.LogInformation($"Anime deleted: {animeId}");
            return new OkObjectResult($"Anime '{animeId}' deleted successfully.");
        }

        [Function("SearchAnime")]
        public async Task<IActionResult> SearchAnime([HttpTrigger(AuthorizationLevel.Function, "get", Route = "animes/search/{animeName}")] HttpRequest req, string animeName)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult(await _jikanService.SearchAnimeAsync(animeName));
        }

        #endregion
    }
}
