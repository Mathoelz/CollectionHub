using CollectionHub.Shared.Dtos;
using System.Net;
using System.Text.Json;

namespace CollectionHub.Components.Services
{
    public class GameApiService: IGameApiService
    {
        private readonly HttpClient _httpClient;

        private readonly List<GameDto> _games = [];

        public GameApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:7029/");

            _games = new List<GameDto>();

        }

        public async Task<List<GameDto>> GetGamesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<GameDto>>("api/GetGames")
                   ?? [];
        }

        public async Task<GameDto> PostGameAsync(GameDto game)
        {
            return await _httpClient.PostAsJsonAsync("api/PostGame", game)
                .ContinueWith(async responseTask =>
                {
                    var response = await responseTask;
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadFromJsonAsync<GameDto>();
                }).Unwrap() ?? new GameDto();
        }

        public async Task<HttpStatusCode> DeleteGameAsync(GameDto game)
        {
            return await _httpClient.DeleteAsync($"api/DeleteGame/{game.Id}")
                .ContinueWith(async responseTask =>
                {
                    var response = await responseTask;
                    return response.StatusCode;
                }).Unwrap();
        }

        public async Task<GameDto> UpdateGameAsync(GameDto game)
        {
            return await _httpClient.PutAsJsonAsync("api/UpdateGame", game)
                .ContinueWith(async responseTask =>
                {
                    var response = await responseTask;
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadFromJsonAsync<GameDto>();
                }).Unwrap() ?? new GameDto();
        }

        public async Task<GameDto> GetGameAsync(GameDto game)
        {
            return await _httpClient.GetFromJsonAsync<GameDto>($"api/GetGame/{game.Id}")
                   ?? new GameDto();
        }
    }
}
