using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Trade balance
    /// </summary>
    [SerializationModel]
    public record WhiteBitTradeBalance
    {
        /// <summary>
        /// Asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// ["<c>available</c>"] Available balance
        /// </summary>
        [JsonPropertyName("available")]
        public decimal Available { get; set; }
        /// <summary>
        /// ["<c>freeze</c>"] Frozen balance
        /// </summary>
        [JsonPropertyName("freeze")]
        public decimal Frozen { get; set; }
    }


}
