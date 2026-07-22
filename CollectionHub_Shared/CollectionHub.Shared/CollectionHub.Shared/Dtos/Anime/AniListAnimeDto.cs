using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CollectionHub.Shared.Dtos.Anime
{
    public class AniListAnimeDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public AniListAnimeTitleDto? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Synopsis { get; set; }

        [JsonPropertyName("coverImage")]
        public AniListAnimeCoverDto? CoverImage { get; set; }

        [JsonPropertyName("averageScore")]
        public int? AverageScore { get; set; }
    }

    public class AniListAnimeTitleDto
    {
        [JsonPropertyName("romaji")]
        public string? Romaji { get; set; }

        [JsonPropertyName("english")]
        public string? English { get; set; }

        [JsonPropertyName("native")]
        public string? Native { get; set; }

        [JsonIgnore]
        public string DisplayTitle =>
            English ?? Romaji ?? Native ?? "Unknown title";
    }

    public class AniListAnimeCoverDto
    {
        [JsonPropertyName("large")]
        public string? Large { get; set; }
    }
}
