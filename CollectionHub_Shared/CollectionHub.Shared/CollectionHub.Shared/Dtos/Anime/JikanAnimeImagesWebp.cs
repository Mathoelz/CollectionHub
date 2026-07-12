using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CollectionHub.Shared.Dtos.Anime
{
    public class JikanAnimeImagesWebp
    {
        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }
        [JsonPropertyName("small_image_url")]
        public string? SmallImageUrl { get; set; }
        [JsonPropertyName("large_image_url")]
        public string? LargeImageUrl { get; set; }
    }
}
