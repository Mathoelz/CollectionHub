using CollectionHub.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionHub.Shared.Dtos.Anime
{
    public class AnimeDto
    {
        public Guid Id { get; set; }

        public string? Title { get; set; }

        public CollectionStatus Status { get; set; }

        public int? Rating { get; set; }

        public string? Notes { get; set; }
        public MediaType MediaType { get; set; } = MediaType.Anime;

    }
}
