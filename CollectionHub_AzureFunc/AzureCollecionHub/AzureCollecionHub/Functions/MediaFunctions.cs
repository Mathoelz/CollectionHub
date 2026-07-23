using CollectionHub.Functions.Services.Anime;
using CollectionHub.Functions.Services.Cosmos;
using CollectionHub.Functions.Services.Covers;
using CollectionHub.Functions.Services.Igdb;
using CollectionHub.Shared.Dtos;
using CollectionHub.Shared.Dtos.Anime;
using CollectionHub.Shared.Dtos.Game;
using CollectionHub.Shared.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace CollectionHub.Functions.Functions
{
    public class MediaFunctions
    {
        private readonly ILogger<MediaFunctions> _logger;
        private readonly IMediaService _mediaService;
        private readonly IGameSearchService _gameSearchService;
        private readonly IAnimeSearchService _animeSearchService;
        private readonly ICoverService _coverService;

        public MediaFunctions(ILogger<MediaFunctions> logger, IMediaService mediaService, IGameSearchService igdbService, IAnimeSearchService animeService, ICoverService coverService)
        {
            _logger = logger;
            _mediaService = mediaService;
            _gameSearchService = igdbService;
            _animeSearchService = animeService;
            _coverService = coverService;
        }

        #region Games

        [Function("GetGames")]
        public async Task<IActionResult> GetGames([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games")] HttpRequest req)
        {
            _logger.LogInformation(
                "Retrieving media collection. MediaType: {MediaType}",
                "Game");

            var games = await _mediaService.GetAllGames();

            _logger.LogInformation(
                "Media collection retrieved. MediaType: {MediaType}, ItemCount: {games.Count}",
                "Game",
                games.Count());

            return new OkObjectResult(games);
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

            GameDto? newGame =
                await req.ReadFromJsonAsync<GameDto>();

            if (newGame is null)
            {
                _logger.LogWarning(
                    "Game creation rejected. Reason: {Reason}",
                    "Missing or invalid request body");

                return new BadRequestObjectResult(
                    "A valid game is required.");
            }

            await _mediaService.AddGame(newGame);

            _logger.LogInformation(
                "Game created. MediaType: {MediaType}, MediaId: {MediaId}",
                "Game",
                newGame.Id);

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

            GameDto? updatedGame =
                await req.ReadFromJsonAsync<GameDto>();

            if (updatedGame is null)
            {
                _logger.LogWarning(
                    "Game update rejected. Reason: {Reason}",
                    "Missing or invalid request body");

                return new BadRequestObjectResult(
                    "A valid game is required.");
            }

            await _mediaService.UpdateGame(updatedGame);

            _logger.LogInformation(
                "Game updated. MediaType: {MediaType}, MediaId: {MediaId}",
                "Game",
                updatedGame.Id);

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

            await _mediaService.DeleteItem(gameId);

            _logger.LogInformation(
                "Game deleted. MediaType: {MediaType}, MediaId: {MediaId}",
                "Game",
                gameId);

            return new OkObjectResult(
                $"Game '{gameId}' deleted successfully.");
        }

        [Function("SearchGames")]
        public async Task<IActionResult> SearchGame([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/search/{gameName}")] HttpRequest req, string gameName)
        {
            if (string.IsNullOrWhiteSpace(gameName))
            {
                return new BadRequestObjectResult(
                    "A game name is required.");
            }

            _logger.LogInformation(
                "Searching IGDB for game: {GameName}",
                gameName);

            var results =
                await _gameSearchService.SearchGamesAsync(gameName);

            _logger.LogInformation(
                "IGDB game search completed. GameName: {GameName}, ResultCount: {ResultCount}",
                gameName,
                results.Count);

            return new OkObjectResult(results);
        }

        [Function("GetGameCover")]
        public async Task<IActionResult> SearchCovers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/covers/{coverId:int}")] HttpRequest req, int coverId)
        {
            var authorizationFailure =
                await AuthorizeWriteRequestAsync(req);

            if (authorizationFailure is not null)
            {
                return authorizationFailure;
            }

            if (coverId <= 0)
            {
                return new BadRequestObjectResult(
                    "A valid game cover ID is required.");
            }

            _logger.LogInformation(
                "Getting game cover. CoverId: {CoverId}",
                coverId);

            string coverUrl =
                await _coverService.GetGameCover(coverId);

            return new OkObjectResult(
                new CoverResponseDto
                {
                    Url = coverUrl
                });
        }

        #endregion

        #region Anime

        [Function("GetAnimes")]
        public async Task<IActionResult> GetAnimes([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "animes")] HttpRequest req)
        {
            _logger.LogInformation(
                "Getting all anime collection entries.");

            var animes = await _mediaService.GetAllAnimes();

            _logger.LogInformation(
                "Anime collection entries retrieved. ResultCount: {ResultCount}",
                animes.Count);

            return new OkObjectResult(animes);
        }

        [Function("GetAnime")]
        public async Task<IActionResult> GetAnime([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "animes/{animeId}")] HttpRequest req, Guid animeId)
        {
            _logger.LogInformation(
                "Getting anime collection entry. AnimeId: {AnimeId}",
                animeId);

            AnimeDto? anime =
                await _mediaService.GetAnimeById(animeId);

            if (anime is null)
            {
                _logger.LogWarning(
                    "Anime collection entry not found. AnimeId: {AnimeId}",
                    animeId);

                return new NotFoundObjectResult(
                    $"Anime '{animeId}' was not found.");
            }

            return new OkObjectResult(anime);
        }

        [Function("AddAnime")]
        public async Task<IActionResult> PostAnime([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "animes")] HttpRequest req, CancellationToken cancellationToken)
        {
            var authorizationFailure =
                await AuthorizeWriteRequestAsync(req);

            if (authorizationFailure is not null)
            {
                return authorizationFailure;
            }

            AnimeDto? newAnime =
                await req.ReadFromJsonAsync<AnimeDto>(
                    cancellationToken);

            if (newAnime is null)
            {
                return new BadRequestObjectResult(
                    "A valid anime is required.");
            }

            newAnime.MediaType = MediaType.Anime;

            await _mediaService.AddAnime(newAnime);

            _logger.LogInformation(
                "Anime collection entry added. AnimeId: {AnimeId}, Title: {Title}",
                newAnime.Id,
                newAnime.Title);

            return new OkObjectResult(newAnime);
        }

        [Function("UpdateAnime")]
        public async Task<IActionResult> UpdateAnime([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "animes")] HttpRequest req, CancellationToken cancellationToken)
        {
            var authorizationFailure =
                await AuthorizeWriteRequestAsync(req);

            if (authorizationFailure is not null)
            {
                return authorizationFailure;
            }

            AnimeDto? updatedAnime =
                await req.ReadFromJsonAsync<AnimeDto>(
                    cancellationToken);

            if (updatedAnime is null ||
                updatedAnime.Id == Guid.Empty)
            {
                return new BadRequestObjectResult(
                    "A valid anime with an ID is required.");
            }

            updatedAnime.MediaType = MediaType.Anime;

            await _mediaService.UpdateAnime(updatedAnime);

            _logger.LogInformation(
                "Anime collection entry updated. AnimeId: {AnimeId}, Title: {Title}",
                updatedAnime.Id,
                updatedAnime.Title);

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

            if (animeId == Guid.Empty)
            {
                return new BadRequestObjectResult(
                    "A valid anime ID is required.");
            }

            await _mediaService.DeleteItem(animeId);

            _logger.LogInformation(
                "Anime collection entry deleted. AnimeId: {AnimeId}",
                animeId);

            return new OkObjectResult(
                $"Anime '{animeId}' deleted successfully.");
        }

        [Function("SearchAnime")]
        public async Task<IActionResult> SearchAnime([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "animes/search/{animeName}")] HttpRequest req, string animeName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(animeName))
            {
                return new BadRequestObjectResult(
                    "An anime name is required.");
            }

            _logger.LogInformation(
                "Searching AniList for anime: {AnimeName}",
                animeName);

            List<AniListAnimeDto> results =
                await _animeSearchService.SearchAnimeAsync(
                    animeName,
                    cancellationToken);

            _logger.LogInformation(
                "AniList anime search completed. AnimeName: {AnimeName}, ResultCount: {ResultCount}",
                animeName,
                results.Count);

            return new OkObjectResult(results);
        }

        [Function("GetAnimeCover")]
        public async Task<IActionResult> GetAnimeCover([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "animes/covers")] HttpRequest req, CancellationToken cancellationToken)
        {
            var authorizationFailure =
                await AuthorizeWriteRequestAsync(req);

            if (authorizationFailure is not null)
            {
                return authorizationFailure;
            }

            AnimeCoverRequestDto? request =
                await req.ReadFromJsonAsync<AnimeCoverRequestDto>(
                    cancellationToken);

            if (request is null ||
                request.AnimeId <= 0 ||
                string.IsNullOrWhiteSpace(request.SourceUrl))
            {
                return new BadRequestObjectResult(
                    "A valid anime ID and cover URL are required.");
            }

            _logger.LogInformation(
                "Getting anime cover. AnimeId: {AnimeId}",
                request.AnimeId);

            string? coverUrl =
                await _coverService.GetAnimeCover(
                    request.AnimeId,
                    request.SourceUrl);

            if (coverUrl is null)
            {
                return new BadRequestObjectResult(
                    "The anime cover URL is invalid or unsupported.");
            }

            return new OkObjectResult(
                new CoverResponseDto
                {
                    Url = coverUrl
                });
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
