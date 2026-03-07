using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Trading fee
    /// </summary>
    [SerializationModel]
    public record WhiteBitTradingFee
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
    }
}
