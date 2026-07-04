namespace CollectionHub.Components.Services
{
    public interface IAnimeService
    {
        IReadOnlyList<Anime> GetAll();

        void Add(Anime anime);

        void Delete(Anime anime);
    }
}
