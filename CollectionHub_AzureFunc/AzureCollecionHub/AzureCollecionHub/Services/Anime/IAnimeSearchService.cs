using CollectionHub.Shared.Dtos.Anime;

namespace CollectionHub.Functions.Services.Anime
{
    public interface IAnimeSearchService
    {
        Task<List<AniListAnimeDto>> SearchAnimeAsync(
            string name,
            CancellationToken cancellationToken = default);
    }
}
