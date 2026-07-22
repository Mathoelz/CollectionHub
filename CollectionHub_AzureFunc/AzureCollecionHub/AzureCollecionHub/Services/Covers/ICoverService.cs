using CollectionHub.Shared.Dtos.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionHub.Functions.Services.Covers
{
    public interface ICoverService
    {
        public Task<string> GetGameCover(int gameId);
        Task<string?> GetAnimeCover(int animeId, string? sourceUrl);
    }
}
