using CollectionHub.Shared.Dtos.Game;
using System.Net;

namespace CollectionHub.Components.Services
{
    public interface IGameApiService
    {
        Task<List<GameDto>> GetGamesAsync();
        Task<GameDto> GetGameAsync(GameDto game);
        Task<GameDto> PostGameAsync(GameDto game);
        Task<GameDto> UpdateGameAsync(GameDto game);
        Task<HttpStatusCode> DeleteGameAsync(GameDto game);
        Task<List<IgdbGameDto>> SearchGames(string gameName);
        Task<IgdbCoverDto> SearchCover(int id);
    }
}
