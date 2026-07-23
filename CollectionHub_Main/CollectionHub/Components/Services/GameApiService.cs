using CollectionHub.Shared.Dtos;
using CollectionHub.Shared.Dtos.Game;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Web;
using System.Net;
using System.Net.Http.Headers;

namespace CollectionHub.Components.Services
{
    public class GameApiService: IGameApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiRetryHandler _retryHandler;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly string _functionsApiScope;

        public GameApiService(
            HttpClient httpClient,
            ApiRetryHandler apiRetryHandler,
            ITokenAcquisition tokenAcquisition,
            AuthenticationStateProvider authenticationStateProvider,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _retryHandler = apiRetryHandler;
            _tokenAcquisition = tokenAcquisition;
            _authenticationStateProvider = authenticationStateProvider;

            _functionsApiScope =
                configuration["FunctionsApi:Scope"]
                ?? throw new InvalidOperationException(
                    "FunctionsApi:Scope is missing.");
        }

        public async Task<List<GameDto>> GetGamesAsync()
        {
            return await _retryHandler.ExecuteAsync(async () =>
            {
                return await _httpClient.GetFromJsonAsync<List<GameDto>>("api/games")
                   ?? [];
            });

        }

        public async Task<GameDto> PostGameAsync(GameDto game)
        {
            using var request =
                await CreateAuthenticatedRequestAsync(
                    HttpMethod.Post,
                    "api/games",
                    JsonContent.Create(game));

            using var response =
                await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<GameDto>()
                ?? throw new InvalidOperationException(
                    "The game API returned an empty response after creating a game.");
        }

        public async Task<HttpStatusCode> DeleteGameAsync(GameDto game)
        {
            return await _retryHandler.ExecuteAsync(async () =>
            {
                using var request =
                    await CreateAuthenticatedRequestAsync(
                        HttpMethod.Delete,
                        $"api/games/{game.Id}");

                using var response =
                    await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return response.StatusCode;
            });
        }

        public async Task<GameDto> UpdateGameAsync(GameDto game)
        {
            return await _retryHandler.ExecuteAsync(async () =>
            {
                using var request =
                    await CreateAuthenticatedRequestAsync(
                        HttpMethod.Put,
                        "api/games",
                        JsonContent.Create(game));

                using var response =
                    await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<GameDto>()
                    ?? new GameDto();
            });
        }

        public async Task<GameDto> GetGameAsync(GameDto game)
        {
            return await _retryHandler.ExecuteAsync(async () =>
            {
                return await _httpClient.GetFromJsonAsync<GameDto>($"api/games/{game.Id}")
                   ?? new GameDto();
            });            
        }

        public async Task<List<IgdbGameDto>> SearchGamesAsync(string gameName)
        {
            return await _retryHandler.ExecuteAsync(async () =>
            {
                return await _httpClient.GetFromJsonAsync<List<IgdbGameDto>>($"api/games/search/{gameName}")
                   ?? [];
            });            
        }

        public async Task<string?> GetGameCoverAsync(int id)
        {
            return await _retryHandler.ExecuteAsync(async () =>
            {
                using var request =
                    await CreateAuthenticatedRequestAsync(
                        HttpMethod.Get,
                        $"api/games/covers/{id}");

                using var response =
                    await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                CoverResponseDto? result =
                    await response.Content
                        .ReadFromJsonAsync<CoverResponseDto>();

                return result?.Url;
            });
        }

        private async Task<HttpRequestMessage> CreateAuthenticatedRequestAsync(
            HttpMethod method,
            string requestUri,
            HttpContent? content = null)
        {
            var authenticationState =
                await _authenticationStateProvider.GetAuthenticationStateAsync();

            if (authenticationState.User.Identity?.IsAuthenticated != true)
            {
                throw new InvalidOperationException(
                    "The user must be authenticated.");
            }

            var accessToken =
                await _tokenAcquisition.GetAccessTokenForUserAsync(
                    [_functionsApiScope],
                    user: authenticationState.User);

            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            return request;
        }
    }
}
