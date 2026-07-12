using CollectionHub.Shared.Dtos.Anime;
using System.Net;

namespace CollectionHub.Components.Services
{
    public interface IAnimeService
    {
        IReadOnlyList<Anime> GetAll();

        void Add(Anime anime);

        void Delete(Anime anime);

        Task<AnimeDto> PostAnimeAsync(AnimeDto anime);
        Task<AnimeDto> UpdateAnimeAsync(AnimeDto anime);
        Task<List<AnimeDto>> GetAnimesAsync();
        Task<HttpStatusCode> DeleteAnimeAsync(AnimeDto anime);
        Task<List<AnimeDto>> SearchAnimesAsync(string name);
    }
}
