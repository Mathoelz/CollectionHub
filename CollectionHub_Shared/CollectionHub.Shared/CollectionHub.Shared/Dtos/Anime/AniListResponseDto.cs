using System.Text.Json.Serialization;

namespace CollectionHub.Shared.Dtos.Anime;

public class AniListResponseDto
{
    [JsonPropertyName("data")]
    public AniListDataDto? Data { get; set; }
}

public class AniListDataDto
{
    [JsonPropertyName("Page")]
    public AniListPageDto? Page { get; set; }
}

public class AniListPageDto
{
    [JsonPropertyName("media")]
    public List<AniListAnimeDto> Media { get; set; } = [];
}