using CollectionHub.Components.Pages.Games;
using CollectionHub.Shared.Dtos.Anime;
using CollectionHub.Shared.Dtos.Game;
using System.Net;

namespace CollectionHub.Components.Services
{
    public class AnimeApiService : IAnimeService
    {
        private readonly HttpClient _httpClient;

        private readonly List<Anime> _animes = new List<Anime>();

        public AnimeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:7029/");

            _animes = new List<Anime>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Frieren: Beyond Journey's End",
                    Status = CollectionStatus.Completed,
                    Rating = 10,
                    Notes = "Beautiful storytelling."
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Vinland Saga",
                    Status = CollectionStatus.Playing,
                    Rating = 9,
                    Notes = "Currently watching Season 2."
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Steins;Gate",
                    Status = CollectionStatus.Backlog,
                    Rating = null,
                    Notes = "Everyone recommends it."
                }
            };
        }

        public IReadOnlyList<Anime> Animes => _animes;

        public async Task<List<AnimeDto>> GetAnimesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<AnimeDto>>("api/animes")
                   ?? new List<AnimeDto>();
        }

        public async Task<AnimeDto> GetAnimeAsync(AnimeDto anime)
        {
            return await _httpClient.GetFromJsonAsync<AnimeDto>($"api/animes/{anime.Id}")
                   ?? new AnimeDto();
        }

        public async Task<AnimeDto> PostAnimeAsync(AnimeDto anime)
        {
            return await _httpClient.PostAsJsonAsync("api/animes", anime)
                .ContinueWith(async responseTask =>
                {
                    var response = await responseTask;
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadFromJsonAsync<AnimeDto>();
                }).Unwrap() ?? new AnimeDto();
        }

        public async Task<HttpStatusCode> DeleteAnimeAsync(AnimeDto anime)
        {
            return await _httpClient.DeleteAsync($"api/animes/{anime.Id}")
                .ContinueWith(async responseTask =>
                {
                    var response = await responseTask;
                    return response.StatusCode;
                }).Unwrap();
        }

        public async Task<AnimeDto> UpdateAnimeAsync(AnimeDto anime)
        {
            return await _httpClient.PutAsJsonAsync("api/animes", anime)
                .ContinueWith(async responseTask =>
                {
                    var response = await responseTask;
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadFromJsonAsync<AnimeDto>();
                }).Unwrap() ?? new AnimeDto();
        }

        public async Task<List<JikanAnimeDto>> SearchAnimesAsync(string name)
        {
            return await _httpClient.GetFromJsonAsync<List<JikanAnimeDto>>($"api/animes/search/{name}")
                   ?? [];
        }
    }
}
