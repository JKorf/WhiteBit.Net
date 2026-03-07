using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Convert result
    /// </summary>
    [SerializationModel]
    public record WhiteBitConvertResult
    {
        /// <summary>
        /// ["<c>finalGive</c>"] Final from quantity
        /// </summary>
        [JsonPropertyName("finalGive")]
        public decimal FinalFromQuantity { get; set; }
        /// <summary>
        /// ["<c>finalReceive</c>"] Final to quantity
        /// </summary>
        [JsonPropertyName("finalReceive")]
        public decimal FinalToQuantity { get; set; }
    }


}
