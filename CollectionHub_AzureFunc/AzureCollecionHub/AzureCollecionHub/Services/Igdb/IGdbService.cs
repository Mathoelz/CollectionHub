using CollectionHub.Functions.Services.Secrets;
using CollectionHub.Shared.Dtos.Game;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace CollectionHub.Functions.Services.Igdb
{
    public class IGdbService : IGameSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly ISecretProvider _secretClient;
        private readonly string _twitchGrantType = Environment.GetEnvironmentVariable("TwitchDeveloperGrantType")!;
        private const string _twitchAuthUrl = "https://id.twitch.tv/oauth2/token";
        private TwitchAuthDto _twitchAuth = new();
        private DateTimeOffset _tokenExpiresAtUtc = DateTimeOffset.MinValue;

        public IGdbService(HttpClient httpClient, ISecretProvider secretClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.igdb.com/v4/");

            _secretClient = secretClient;
        }

        private async Task RefreshTokenAsync()
        {
            var twitchClientId = await _secretClient.GetSecretAsync("TwitchDeveloperClientId");
            var twitchClientSecret = await _secretClient.GetSecretAsync("TwitchDeveloperClientSecret");

            var authUrl =   $"{_twitchAuthUrl}" +
                            $"?client_id={twitchClientId}" +
                            $"&client_secret={twitchClientSecret}" +
                            $"&grant_type={_twitchGrantType}";

            var response = await _httpClient.PostAsync(authUrl, null);

            response.EnsureSuccessStatusCode();

            _twitchAuth =
                await response.Content.ReadFromJsonAsync<TwitchAuthDto>()
                ?? throw new InvalidOperationException(
                    "The Twitch authentication response was empty.");

            var tokenLifetime =
                Math.Max(0, _twitchAuth.ExpiresIn - 60);

            _tokenExpiresAtUtc =
                DateTimeOffset.UtcNow.AddSeconds(tokenLifetime);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", twitchClientId);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _twitchAuth.AccessToken);
        }

        private async Task EnsureTokenAsync()
        {
            if (string.IsNullOrWhiteSpace(_twitchAuth.AccessToken) ||
                DateTimeOffset.UtcNow >= _tokenExpiresAtUtc)
            {
                await RefreshTokenAsync();
            }
        }

        public async Task<List<IgdbGameDto>> SearchGamesAsync(string search)
        {
            await EnsureTokenAsync();

            var query =
                $"""
                search "{search}";
                fields name, summary, cover, rating, first_release_date;
                limit 10;
                """;

            var content = new StringContent(
                query,
                Encoding.UTF8,
                "text/plain"
            );

            var response = await _httpClient.PostAsync("games", content);

            response.EnsureSuccessStatusCode();

            List<IgdbGameDto> games = await response.Content.ReadFromJsonAsync<List<IgdbGameDto>>() ?? [];

            return games;
        }

        public async Task<IgdbCoverDto> GetCoverAsync(int id)
        {
            await EnsureTokenAsync();

            var query =
                $"""
                fields url, image_id, height, width, game;
                where id = {id};
                """;

            var content = new StringContent(
                query,
                Encoding.UTF8,
                "text/plain"
            );

            var response = await _httpClient.PostAsync("covers", content);

            response.EnsureSuccessStatusCode();

            List<IgdbCoverDto> covers = await response.Content.ReadFromJsonAsync<List<IgdbCoverDto>>() ?? [];

            return covers.FirstOrDefault()
                ?? throw new InvalidOperationException(
                    $"IGDB returned no cover for ID {id}.");
        }  

    }
}
