using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Internal
{
    [SerializationModel]
    internal record WhiteBitSocketUpdate<T>
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }
        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty;
        [JsonPropertyName("params")]
        public T? Data { get; set; } = default!;
    }
}
