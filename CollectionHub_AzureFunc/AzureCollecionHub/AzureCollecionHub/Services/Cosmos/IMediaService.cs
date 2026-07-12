using CollectionHub.Shared.Dtos.Anime;
using CollectionHub.Shared.Dtos.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionHub.Functions.Services.Cosmos
{
    public interface IMediaService
    {
        public Task<List<GameDto>> GetAllGames();
        public Task<GameDto> GetGameById(Guid id);
        public Task UpdateGame(GameDto game);
        public Task<GameDto> EditGame(GameDto game);
        public Task DeleteItem(Guid id);
        public Task<List<AnimeDto>> GetAllAnimes();
        public Task<AnimeDto> GetAnimeById(Guid id);
        public Task AddGame(GameDto game);
        public Task AddAnime(AnimeDto anime);
        public Task UpdateAnime(AnimeDto anime);
        public Task<AnimeDto> EditAnime(AnimeDto anime);
    }
}
