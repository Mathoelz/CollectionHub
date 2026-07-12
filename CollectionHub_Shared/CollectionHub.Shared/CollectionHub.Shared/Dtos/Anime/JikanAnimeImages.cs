using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CollectionHub.Shared.Dtos.Anime
{
    public class JikanAnimeImages
    {
        [JsonPropertyName("jpg")]
        public JikanAnimeImagesJpg? Jpg { get; set; }
        [JsonPropertyName("webp")]
        public JikanAnimeImagesWebp? Webp { get; set; }
    }
}
