using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Sub balances
    /// </summary>
    [SerializationModel]
    public record WhiteBitSubBalances
    {
        /// <summary>
        /// Asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// ["<c>main</c>"] Main
        /// </summary>
        [JsonPropertyName("main")]
        public decimal Main { get; set; }
        /// <summary>
        /// ["<c>spot</c>"] Spot
        /// </summary>
        [JsonPropertyName("spot")]
        public decimal Spot { get; set; }
        /// <summary>
        /// ["<c>collateral</c>"] Collateral
        /// </summary>
        [JsonPropertyName("collateral")]
        public decimal Collateral { get; set; }
    }
}
