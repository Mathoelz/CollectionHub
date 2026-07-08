using CollectionHub.Shared.Dtos;
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
    }
}
