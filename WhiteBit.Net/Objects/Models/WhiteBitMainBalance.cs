using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Balance
    /// </summary>
    [SerializationModel]
    public record WhiteBitMainBalance
    {
        /// <summary>
        /// Asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>main_balance</c>"] Main balance
        /// </summary>
        [JsonPropertyName("main_balance")]
        public decimal MainBalance { get; set; }
    }
}
