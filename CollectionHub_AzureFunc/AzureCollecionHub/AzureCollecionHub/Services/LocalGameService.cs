using CollectionHub.Shared.Dtos.Game;

namespace CollectionHub.Functions.Services
{
    public class LocalGameService
    {
        private List<GameDto> _games =
        [
        new()
                {
                    Id = Guid.Parse("6cb73441-87a3-4da0-9ef0-ca2c76e5f1e9"),
                    Title = "Persona 5 Royal",
                    Status = CollectionStatus.Completed,
                    Rating = 10,
                    Notes = "One of my all-time favorites."
                },

                new()
                {
                    Id = Guid.Parse("e825fc93-bd89-4ff8-b63f-eeb4f4d3aa8e"),
                    Title = "The Legend of Zelda: Tears of the Kingdom",
                    Status = CollectionStatus.Completed,
                    Rating = 10,
                    Notes = "Fantastic exploration and gameplay."
                },

                new()
                {
                    Id = Guid.Parse("bf8d68d9-d6a5-429d-9c88-aae75a1de3ad"),
                    Title = "NieR: Automata",
                    Status = CollectionStatus.Completed,
                    Rating = 10,
                    Notes = "Amazing story and soundtrack."
                },

                new()
                {
                    Id = Guid.Parse("d12634cf-b0da-4fa8-93ad-f269e66eaa35"),
                    Title = "Clair Obscur: Expedition 33",
                    Status = CollectionStatus.Playing,
                    Rating = 10,
                    Notes = "Absolutely amazing so far."
                },

                new()
                {
                    Id = Guid.Parse("269c08b9-9cd1-420c-8b31-fd48844bd9d8"),
                    Title = "Hollow Knight",
                    Status = CollectionStatus.Backlog,
                    Rating = null,
                    Notes = "Need to finally start it."
                },

                new()
                {
                    Id = Guid.Parse("d0b99c78-661d-4a02-a090-c39cdbb54a32"),
                    Title = "Metaphor: ReFantazio",
                    Status = CollectionStatus.Backlog,
                    Rating = null,
                    Notes = "Looking forward to playing it."
                },

                new()
                {
                    Id = Guid.Parse("657ecbf1-4f46-4454-a5c3-2cb03375af29"),
                    Title = "Elden Ring",
                    Status = CollectionStatus.Backlog,
                    Rating = null,
                    Notes = "Still waiting for the right moment."
                }
    ];

        public async Task<List<GameDto>> GetAllGames()
        {
            return _games;
        }

        public async Task<GameDto> GetGameById(Guid id)
        {
            return _games.FirstOrDefault(g => g.Id == id) ?? new GameDto();
        }

        public async Task UpdateGame(GameDto game)
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

        public async Task<GameDto> EditGame(GameDto game)
        {
            GameDto editGame = new()
            {
                Id = game.Id,
                Title = game.Title,
                Status = game.Status,
                Rating = game.Rating,
                Notes = game.Notes
            };

            return editGame;
        }

        public async Task DeleteItem(Guid id)
        {
            var delete = _games.FirstOrDefault(g => g.Id == id);

            if(delete != null)
                _games.Remove(delete);
        }

        public async Task AddGame(GameDto game)
        {
            _games.Add(game);
        }
    }
}
