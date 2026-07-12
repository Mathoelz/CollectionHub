using CollectionHub.Shared.Dtos.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CollectionHub.Functions.Services.Igdb
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

            List<IgdbGameDto> games = await response.Content.ReadFromJsonAsync<List<IgdbGameDto>>();

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

            List<IgdbCoverDto> covers = await response.Content.ReadFromJsonAsync<List<IgdbCoverDto>>();

            return covers.FirstOrDefault();
        }  

    }
}
