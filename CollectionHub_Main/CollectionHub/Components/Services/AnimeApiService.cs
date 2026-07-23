using CollectionHub.Shared.Dtos;
using CollectionHub.Shared.Dtos.Anime;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Web;
using System.Net;
using System.Net.Http.Headers;

namespace CollectionHub.Components.Services
{
    public class AnimeApiService : IAnimeService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiRetryHandler _retryHandler;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly string _functionsApiScope;

        public AnimeApiService(
            HttpClient httpClient,
            ApiRetryHandler retryHandler,
            ITokenAcquisition tokenAcquisition,
            AuthenticationStateProvider authenticationStateProvider,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _retryHandler = retryHandler;
            _tokenAcquisition = tokenAcquisition;
            _authenticationStateProvider = authenticationStateProvider;

            _functionsApiScope =
                configuration["FunctionsApi:Scope"]
                ?? throw new InvalidOperationException(
                    "FunctionsApi:Scope is missing.");
        }

        private async Task<HttpRequestMessage>
            CreateAuthenticatedRequestAsync(
                HttpMethod method,
                string requestUri,
                HttpContent? content = null)
        {
            var authenticationState =
                await _authenticationStateProvider
                    .GetAuthenticationStateAsync();

            if (authenticationState.User.Identity?.IsAuthenticated
                != true)
            {
                throw new InvalidOperationException(
                    "The user must be authenticated.");
            }

            var accessToken =
                await _tokenAcquisition.GetAccessTokenForUserAsync(
                    [_functionsApiScope],
                    user: authenticationState.User);

            var request = new HttpRequestMessage(
                method,
                requestUri)
            {
                Content = content
            };

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    accessToken);

            return request;
        }

        public async Task<List<AnimeDto>> GetAnimesAsync()
        {
            return await _retryHandler.ExecuteAsync(async () =>
            {
                return await _httpClient
                    .GetFromJsonAsync<List<AnimeDto>>(
                        "api/animes")
                    ?? [];
            });
        }

        public async Task<AnimeDto> GetAnimeAsync(AnimeDto anime)
        {
            return await _retryHandler.ExecuteAsync(async () =>
            {
                return await _httpClient
                    .GetFromJsonAsync<AnimeDto>(
                        $"api/animes/{anime.Id}")
                    ?? new AnimeDto();
            });
        }

        public async Task<AnimeDto> PostAnimeAsync(AnimeDto anime)
        {
            return await _retryHandler.ExecuteAsync(async () =>
            {
                using var request =
                    await CreateAuthenticatedRequestAsync(
                        HttpMethod.Post,
                        "api/animes",
                        JsonContent.Create(anime));

                using var response =
                    await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return await response.Content
                    .ReadFromJsonAsync<AnimeDto>()
                    ?? throw new InvalidOperationException(
                    "The anime API returned an empty response after creating an anime.");
            });
        }

        public async Task<HttpStatusCode> DeleteAnimeAsync(AnimeDto anime)
        {
            return await _retryHandler.ExecuteAsync(async () =>
            {
                using var request =
                    await CreateAuthenticatedRequestAsync(
                        HttpMethod.Delete,
                        $"api/animes/{anime.Id}");

                using var response =
                    await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return response.StatusCode;
            });
        }

        public async Task<AnimeDto> UpdateAnimeAsync(AnimeDto anime)
        {
            return await _retryHandler.ExecuteAsync(async () =>
            {
                using var request =
                    await CreateAuthenticatedRequestAsync(
                        HttpMethod.Put,
                        "api/animes",
                        JsonContent.Create(anime));

                using var response =
                    await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return await response.Content
                    .ReadFromJsonAsync<AnimeDto>()
                    ?? new AnimeDto();
            });
        }

        public async Task<List<AniListAnimeDto>>SearchAnimesAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return [];
            }

            string encodedName = Uri.EscapeDataString(name.Trim());

            return await _retryHandler.ExecuteAsync(async () =>
            {
                return await _httpClient
                    .GetFromJsonAsync<List<AniListAnimeDto>>(
                        $"api/animes/search/{encodedName}")
                    ?? [];
            });
        }

        public async Task<string?> GetAnimeCoverAsync(int animeId, string? sourceUrl)
        {
            var coverRequest = new AnimeCoverRequestDto
            {
                AnimeId = animeId,
                SourceUrl = sourceUrl
            };

            return await _retryHandler.ExecuteAsync(async () =>
            {
                using var request =
                    await CreateAuthenticatedRequestAsync(
                        HttpMethod.Post,
                        "api/animes/covers",
                        JsonContent.Create(coverRequest));

                using var response =
                    await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                CoverResponseDto? result =
                    await response.Content
                        .ReadFromJsonAsync<CoverResponseDto>();

                return result?.Url;
            });
        }
    }
}