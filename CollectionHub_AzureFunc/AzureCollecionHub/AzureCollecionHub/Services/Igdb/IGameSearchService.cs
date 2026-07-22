using CollectionHub.Shared.Dtos.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionHub.Functions.Services.Igdb
{
    public interface IGameSearchService
    {
        public Task<List<IgdbGameDto>> SearchGamesAsync(string search);
        public Task<IgdbCoverDto> GetCoverAsync(int id);
    }
}
