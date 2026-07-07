using CollectionHub.Shared.Dtos;

namespace CollectionHub.Components.Services
{
    public interface IGameService
    {
        Task<List<GameDto>> GetGamesAsync();
        IReadOnlyList<GameDto> GetAll();
        void Add(GameDto game);
        void Delete(GameDto game);
        void Update(GameDto game);
        GameDto Edit(GameDto game);
    }
}
