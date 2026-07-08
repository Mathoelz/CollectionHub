using CollectionHub.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionHub.Functions.Services
{
    public interface IGameService
    {
        public List<GameDto> GetAll();
        public GameDto GetById(Guid id);
        public void Update(GameDto game);
        public GameDto Edit(GameDto game);
        public void Delete(Guid id);
        public void Add(GameDto game);
    }
}
