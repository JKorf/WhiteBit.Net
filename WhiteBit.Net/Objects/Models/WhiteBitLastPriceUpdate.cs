using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Last price update
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<WhiteBitLastPriceUpdate>))]
    [SerializationModel]
    public record WhiteBitLastPriceUpdate
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [ArrayProperty(0)]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Last price
        /// </summary>
        [ArrayProperty(1), JsonConverter(typeof(DecimalConverter))]
        public decimal LastPrice { get; set; } 
    }
}
