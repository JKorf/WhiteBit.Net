using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Book Ticker update
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<WhiteBitBookTickerUpdate>))]
    [SerializationModel]
    public record WhiteBitBookTickerUpdate
    {
        /// <summary>
        /// Timestamp of the data
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(DateTimeConverter))]
        public DateTime DataTime { get; set; }
        /// <summary>
        /// Timestamp of the message
        /// </summary>
        [ArrayProperty(1), JsonConverter(typeof(DateTimeConverter))]
        public DateTime MessageTime { get; set; }
        /// <summary>
        /// Symbol
        /// </summary>
        [ArrayProperty(2)]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Update id
        /// </summary>
        [ArrayProperty(3)]
        public long UpdateId { get; set; }
        /// <summary>
        /// Best bid price
        /// </summary>
        [ArrayProperty(4), JsonConverter(typeof(DecimalConverter))]
        public decimal BestBidPrice { get; set; }
        /// <summary>
        /// Best bid quantity
        /// </summary>
        [ArrayProperty(5), JsonConverter(typeof(DecimalConverter))]
        public decimal BestBidQuantity { get; set; }
        /// <summary>
        /// Best ask price
        /// </summary>
        [ArrayProperty(6), JsonConverter(typeof(DecimalConverter))]
        public decimal BestAskPrice { get; set; }
        /// <summary>
        /// Best ask price
        /// </summary>
        [ArrayProperty(7), JsonConverter(typeof(DecimalConverter))]
        public decimal BestAskQuantity { get; set; }
    }
}
