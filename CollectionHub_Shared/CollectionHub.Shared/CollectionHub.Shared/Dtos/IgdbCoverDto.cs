using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CollectionHub.Shared.Dtos
{
    public class IgdbCoverDto
    {
        [JsonPropertyName("game")]
        public int GameId { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
        [JsonPropertyName("height")]
        public int Height { get; set; }
        [JsonPropertyName("width")]
        public int Width { get; set; }
        [JsonPropertyName("image_id")]
        public string? ImageId { get; set; }
    }
}
