#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    public class EmbedField
    {
        [JsonProperty("index")]
        public int Index { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("inline")]
        public bool Inline { get; set; }
    }
}
