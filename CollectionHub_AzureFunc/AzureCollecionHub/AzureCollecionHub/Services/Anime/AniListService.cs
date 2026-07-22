using System.Net.Http.Json;
using CollectionHub.Shared.Dtos.Anime;

namespace CollectionHub.Functions.Services.Anime;

public class AniListService : IAnimeSearchService
{
    private readonly HttpClient _httpClient;

    public AniListService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://graphql.anilist.co/");
    }

    public async Task<List<AniListAnimeDto>> SearchAnimeAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        const string query = """
            query ($search: String!) {
              Page(page: 1, perPage: 10) {
                media(search: $search, type: ANIME) {
                  id
                  title {
                    romaji
                    english
                    native
                  }
                  description
                  coverImage {
                    large
                  }
                  averageScore
                }
              }
            }
            """;

        var request = new
        {
            query,
            variables = new
            {
                search = name
            }
        };

        using HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "",
                request,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        AniListResponseDto? result =
            await response.Content.ReadFromJsonAsync<AniListResponseDto>(
                cancellationToken: cancellationToken);

        return result?.Data?.Page?.Media ?? [];
    }
}