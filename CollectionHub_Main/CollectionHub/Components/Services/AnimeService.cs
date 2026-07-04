namespace CollectionHub.Components.Services
{
    public class AnimeService : IAnimeService
    {
        private readonly List<Anime> _animes = new List<Anime>();

        public AnimeService()
        {
            _animes = new List<Anime>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Frieren: Beyond Journey's End",
                    Status = CollectionStatus.Completed,
                    Rating = 10,
                    Notes = "Beautiful storytelling."
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Vinland Saga",
                    Status = CollectionStatus.Playing,
                    Rating = 9,
                    Notes = "Currently watching Season 2."
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Steins;Gate",
                    Status = CollectionStatus.Backlog,
                    Rating = null,
                    Notes = "Everyone recommends it."
                }
            };
        }

        public IReadOnlyList<Anime> Animes => _animes;
        public IReadOnlyList<Anime> GetAll()
        {
            return _animes;
        }

        public void Add(Anime anime)
        {
            _animes.Add(anime);
        }

        public void Delete(Anime anime)
        {
            _animes.Remove(anime);
        }

        
    }
}
