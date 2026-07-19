using Azure.Security.KeyVault.Secrets;
using CollectionHub.Functions.Services.Anime;
using CollectionHub.Functions.Services.Cosmos;
using CollectionHub.Functions.Services.Covers;
using CollectionHub.Functions.Services.Igdb;
using CollectionHub.Shared.Dtos.Anime;
using CollectionHub.Shared.Dtos.Game;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
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
        private readonly ICoverService _coverService;

        public MediaFunctions(ILogger<MediaFunctions> logger, IMediaService mediaService, IGdbService igdbService, JikanService jikanService, ICoverService coverService)
        {
            _logger = logger;
            _mediaService = mediaService;
            _igdbService = igdbService;
            _jikanService = jikanService;
            _coverService = coverService;
        }

        #region Games

        [Function("GetGames")]
        public async Task<IActionResult> GetGames([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(await _mediaService.GetAllGames());
        }

        [Function("GetGame")]
        public async Task<IActionResult> GetGame([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}")] HttpRequest req, Guid gameId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");


            return new OkObjectResult(await _mediaService.GetGameById(gameId));
        }

        [Function("PostGames")]
        public async Task<IActionResult> PostGame(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "games")]
            HttpRequest req)
        {
            var authorizationFailure =
                await AuthorizeWriteRequestAsync(req);

            if (authorizationFailure is not null)
            {
                return authorizationFailure;
            }

            _logger.LogInformation(
                "Authenticated user is adding a game.");

            GameDto? newGame =
                await req.ReadFromJsonAsync<GameDto>();

            if (newGame is null)
            {
                return new BadRequestObjectResult(
                    "A valid game is required.");
            }

            await _mediaService.AddGame(newGame);

            _logger.LogInformation(
                "New game added: {GameTitle}",
                newGame.Title);

            return new OkObjectResult(newGame);
        }

        [Function("UpdateGames")]
        public async Task<IActionResult> UpdateGame([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "games")] HttpRequest req)
        {
            var authorizationFailure =
                await AuthorizeWriteRequestAsync(req);

            if (authorizationFailure is not null)
            {
                return authorizationFailure;
            }

            _logger.LogInformation("C# HTTP trigger function processed a request.");
            GameDto updatedGame = await req.ReadFromJsonAsync<GameDto>();

            if(updatedGame != null)
            {
                await _mediaService.UpdateGame(updatedGame);
                _logger.LogInformation($"Game updated: {updatedGame.Title}");
            }
            return new OkObjectResult(updatedGame);
        }

        [Function("DeleteGames")]
        public async Task<IActionResult> DeleteGame([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "games/{gameId}")] HttpRequest req, Guid gameId)
        {
            var authorizationFailure =
                await AuthorizeWriteRequestAsync(req);

            if (authorizationFailure is not null)
            {
                return authorizationFailure;
            }

            _logger.LogInformation("C# HTTP trigger function processed a request.");

            await _mediaService.DeleteItem(gameId);
            _logger.LogInformation($"Game deleted: {gameId}");
            
            return new OkObjectResult($"Game '{gameId}' deleted successfully.");
        }

        [Function("SearchGames")]
        public async Task<IActionResult> SearchGame([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/search/{gameName}")] HttpRequest req, string gameName)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
          
            return new OkObjectResult(await _igdbService.SearchGamesAsync(gameName));
        }

        [Function("SearchCovers")]
        public async Task<IActionResult> SearchCovers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/covers/{gameId}")] HttpRequest req, int gameId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(await _coverService.GetCover(gameId));
        }

        #endregion

        #region Anime

        [Function("GetAnimes")]
        public async Task<IActionResult> GetAnimes([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "animes")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult(await _mediaService.GetAllAnimes());
        }

        [Function("GetAnime")]
        public async Task<IActionResult> GetAnime([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "animes/{animeId}")] HttpRequest req, Guid animeId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult(await _mediaService.GetAnimeById(animeId));
        }

        [Function("AddAnime")]
        public async Task<IActionResult> PostAnime([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "animes")] HttpRequest req)
        {
            var authorizationFailure =
                await AuthorizeWriteRequestAsync(req);

            if (authorizationFailure is not null)
            {
                return authorizationFailure;
            }

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
        public async Task<IActionResult> UpdateAnime([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "animes")] HttpRequest req)
        {
            var authorizationFailure =
                await AuthorizeWriteRequestAsync(req);

            if (authorizationFailure is not null)
            {
                return authorizationFailure;
            }

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
        public async Task<IActionResult> DeleteAnime([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "animes/{animeId}")] HttpRequest req, Guid animeId)
        {
            var authorizationFailure =
                await AuthorizeWriteRequestAsync(req);

            if (authorizationFailure is not null)
            {
                return authorizationFailure;
            }

            _logger.LogInformation("C# HTTP trigger function processed a request.");
            await _mediaService.DeleteItem(animeId);
            _logger.LogInformation($"Anime deleted: {animeId}");
            return new OkObjectResult($"Anime '{animeId}' deleted successfully.");
        }

        [Function("SearchAnime")]
        public async Task<IActionResult> SearchAnime([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "animes/search/{animeName}")] HttpRequest req, string animeName)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult(await _jikanService.SearchAnimeAsync(animeName));
        }

        #endregion

        private async Task<IActionResult?> AuthorizeWriteRequestAsync(
            HttpRequest req)
        {
            var (isAuthenticated, authenticationResponse) =
                await req.HttpContext.AuthenticateAzureFunctionAsync();

            if (!isAuthenticated)
            {
                return authenticationResponse
                    ?? new UnauthorizedResult();
            }

            try
            {
                req.HttpContext.VerifyUserHasAnyAcceptedScope(
                    "access_as_user");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Authenticated user does not have the required scope.");

                return new ForbidResult();
            }

            return null;
        }
    }
}
