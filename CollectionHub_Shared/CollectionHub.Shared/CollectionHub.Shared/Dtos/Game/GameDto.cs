using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CollectionHub.Shared.Dtos.Game
{
    public class GameDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public CollectionStatus Status { get; set; }

        public int? Rating { get; set; }

        public string? Notes { get; set; }
        public string? FirstReleaseDate { get; set; }
        public IgdbCoverDto? Cover { get; set; } = new();
    }
}
