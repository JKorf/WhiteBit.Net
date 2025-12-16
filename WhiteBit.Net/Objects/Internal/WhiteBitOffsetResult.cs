using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Internal
{
    [SerializationModel]
    internal record WhiteBitOffsetResult<T>
    {
        [JsonPropertyName("records")]
        public T[] Records { get; set; } = [];
    }
}
