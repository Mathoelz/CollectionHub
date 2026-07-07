using CollectionHub.Shared.Dtos;
using System.Text.Json;

namespace CollectionHub.Components.Services
{
    public class GameApiService: IGameService
    {
        private readonly HttpClient _httpClient;

        private readonly List<GameDto> _games = [];

        public GameApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:7029/");

            _games = new List<GameDto>();

        }

        public IReadOnlyList<GameDto> Games => _games;

        public IReadOnlyList<GameDto> GetAll()
        {
            return _games;
        }

        public void Add(GameDto game)
        {
            _games.Add(game);
        }

        public void Delete(GameDto game) 
        {
            _games.Remove(game);
        }

        public void Update(GameDto game)
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
        
        public GameDto Edit(GameDto game)
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

        public async Task<List<GameDto>> GetGamesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<GameDto>>("api/GetGames")
                   ?? [];
        }
    }
}
