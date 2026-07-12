using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CollectionHub.Shared.Dtos.Anime;

namespace CollectionHub.Functions.Services.Anime
{
    public class JikanService
    {
        private readonly HttpClient _httpClient;

        public JikanService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.jikan.moe/v4/");
        }

        public async Task<List<JikanAnimeDto>> SearchAnimeAsync(string name)
        {
            var response = await _httpClient.GetAsync($"anime?q={name}&limit=10");

            response.EnsureSuccessStatusCode();

            List<JikanAnimeDto> animeList = await response.Content.ReadFromJsonAsync<List<JikanAnimeDto>>();

            return animeList;
        }
    }
}
