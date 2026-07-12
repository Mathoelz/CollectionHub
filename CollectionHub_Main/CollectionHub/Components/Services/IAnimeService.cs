using CollectionHub.Shared.Dtos.Anime;
using System.Net;

namespace CollectionHub.Components.Services
{
    public interface IAnimeService
    {
        Task<AnimeDto> PostAnimeAsync(AnimeDto anime);
        Task<AnimeDto> UpdateAnimeAsync(AnimeDto anime);
        Task<List<AnimeDto>> GetAnimesAsync();
        Task<AnimeDto> GetAnimeAsync(AnimeDto anime);
        Task<HttpStatusCode> DeleteAnimeAsync(AnimeDto anime);
        Task<List<JikanAnimeDto>> SearchAnimesAsync(string name);
    }
}
