using CollectionHub.Shared.Dtos.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionHub.Functions.Services
{
    public interface IGameService
    {
        public Task<List<GameDto>> GetAll();
        public Task<GameDto> GetById(Guid id);
        public Task Update(GameDto game);
        public Task<GameDto> Edit(GameDto game);
        public Task Delete(Guid id);
        public Task Add(GameDto game);
    }
}
