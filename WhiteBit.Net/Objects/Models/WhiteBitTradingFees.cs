using CryptoExchange.Net.Converters.SystemTextJson;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Trading fees
    /// </summary>
    [SerializationModel]
    public record WhiteBitTradingFees
    {
        /// <summary>
        /// ["<c>taker</c>"] Taker fee rate
        /// </summary>
        [JsonPropertyName("taker")]
        public decimal TakerFee { get; set; }
        /// <summary>
        /// ["<c>maker</c>"] Maker fee rate
        /// </summary>
        [JsonPropertyName("maker")]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// ["<c>custom_fee</c>"] Fees for symbols which are different from the default fees
        /// </summary>
        [JsonPropertyName("custom_fee")]
        public Dictionary<string, WhiteBitTradingFee> CustomFees { get; set; } = new Dictionary<string, WhiteBitTradingFee>();
    }
}
