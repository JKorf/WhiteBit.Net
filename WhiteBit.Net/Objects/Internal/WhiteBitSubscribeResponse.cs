using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Internal
{
    [SerializationModel]
    internal record WhiteBitSubscribeResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}
