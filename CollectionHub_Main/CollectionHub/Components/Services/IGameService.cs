namespace CollectionHub.Components.Services
{
    public interface IGameService
    {
        IReadOnlyList<Game> GetAll();
        void Add(Game game);
        void Delete(Game game);
        void Update(Game game);
        Game Edit(Game game);
    }
}
