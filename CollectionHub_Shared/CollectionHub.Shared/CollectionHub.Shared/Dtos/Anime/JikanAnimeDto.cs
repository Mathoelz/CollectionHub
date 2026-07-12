using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CollectionHub.Shared.Dtos.Anime
{
    public class JikanAnimeDto
    {
        [JsonPropertyName("titles")]
        public JikanAnimeTitle[]? Title { get; set; }
        [JsonPropertyName("synopis")]
        public string? Synopsis { get; set; }
        [JsonPropertyName("images")]
        public JikanAnimeImages? Images { get; set; }
        [JsonPropertyName("score")]
        public float? Score { get; set; }
    }
}
