using CollectionHub.Shared.Enums;

namespace CollectionHub.Shared.Dtos.Anime
{
    public class AnimeDto
    {
        public Guid Id { get; set; }

        public string? Title { get; set; }

        public CollectionStatus Status { get; set; }

        public int? Rating { get; set; }

        public string? Notes { get; set; }
        public string? Cover { get; set; }
        public MediaType MediaType { get; set; } = MediaType.Anime;

    }
}
