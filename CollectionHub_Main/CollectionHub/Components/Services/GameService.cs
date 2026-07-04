namespace CollectionHub.Components.Services
{
    public class GameService: IGameService
    {
        private readonly List<Game> _games = [];

        public GameService()
        {
            _games =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Clair Obscur: Expedition 33",
                    Status = CollectionStatus.Playing,
                    Rating = 10,
                    Notes = "Absolutely amazing so far."
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Persona 5 Royal",
                    Status = CollectionStatus.Completed,
                    Rating = 10,
                    Notes = "One of my all-time favorites."
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Hollow Knight",
                    Status = CollectionStatus.Backlog,
                    Rating = null,
                    Notes = "Need to finally start it."
                }
            ];
        }

        public IReadOnlyList<Game> Games => _games;

        public IReadOnlyList<Game> GetAll()
        {
            return _games;
        }

        public void Add(Game game)
        {
            _games.Add(game);
        }

        public void Delete(Game game) 
        {
            _games.Remove(game);
        }

        public void Update(Game game)
        {
            var existingGame = _games.FirstOrDefault(g => g.Id == game.Id);
            if (existingGame != null)
            {
                existingGame.Title = game.Title;
                existingGame.Status = game.Status;
                existingGame.Rating = game.Rating;
                existingGame.Notes = game.Notes;
            }
        }
        
        public Game Edit(Game game)
        {
            Game editGame = new()
            {
                Id = game.Id,
                Title = game.Title,
                Status = game.Status,
                Rating = game.Rating,
                Notes = game.Notes
            };

            return editGame;
        }
    }
}
