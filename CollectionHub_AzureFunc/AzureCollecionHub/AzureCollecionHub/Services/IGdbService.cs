using CollectionHub.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CollectionHub.Functions.Services
{
    public class IGdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _twitchClientId = Environment.GetEnvironmentVariable("TwitchDeveloperClientId")!;
        private readonly string _twitchClientSecret = Environment.GetEnvironmentVariable("TwitchDeveloperClientSecret")!;
        private readonly string _twitchGrantType = Environment.GetEnvironmentVariable("TwitchDeveloperGrantType")!;
        private const string _twitchAuthUrl = "https://id.twitch.tv/oauth2/token";
        private TwitchAuthDto _twitchAuth = new();

        public IGdbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.igdb.com/v4/");   
        }

        private async Task RefreshTokenAsync()
        {
            var authUrl =   $"{_twitchAuthUrl}" +
                            $"?client_id={_twitchClientId}" +
                            $"&client_secret={_twitchClientSecret}" +
                            $"&grant_type={_twitchGrantType}";

            var response = await _httpClient.PostAsync(authUrl, null);

            response.EnsureSuccessStatusCode();

            _twitchAuth = await response.Content.ReadFromJsonAsync<TwitchAuthDto>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", _twitchClientId);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _twitchAuth.AccessToken);
        }

        private async Task EnsureTokenAsync()
        {
            if (string.IsNullOrWhiteSpace(_twitchAuth.AccessToken) || _twitchAuth.ExpiresIn <= 0)
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
                fields name, summary, cover, rating;
                limit 10;
                """;

            var content = new StringContent(
                query,
                Encoding.UTF8,
                "text/plain"
            );

            var response = await _httpClient.PostAsync("games", content);

            response.EnsureSuccessStatusCode();

            List<IgdbGameDto> games = await response.Content.ReadFromJsonAsync<List<IgdbGameDto>>();

            return games;
        }

        public async Task GetCoverAsync(int id)
        {

        }

        public async Task GetGameAsync(int id)
        {

        }

        private GameDto MapToGame()
        {
            return new GameDto
            {
                Id = Guid.NewGuid(),
                Title = "Sample Game",
                Status = CollectionStatus.Backlog,
                Rating = 0,
                Notes = "This is a sample game."
            };
        }
    }
}
