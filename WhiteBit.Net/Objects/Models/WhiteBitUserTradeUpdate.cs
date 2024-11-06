using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// User trade
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public record WhiteBitUserTradeUpdate
    {
        /// <summary>
        /// Trade id
        /// </summary>
        [ArrayProperty(0)]
        public long Id { get; set; }
        /// <summary>
        /// Trade time
        /// </summary>
        [ArrayProperty(1)]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Time { get; set; }
        /// <summary>
        /// Symbol
        /// </summary>
        [ArrayProperty(2)]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Order id
        /// </summary>
        [ArrayProperty(3)]
        public long OrderId { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        [ArrayProperty(4)]
        public decimal Price { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        [ArrayProperty(5)]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Fee
        /// </summary>
        [ArrayProperty(6)]
        public decimal Fee { get; set; }
        /// <summary>
        /// Client order id
        /// </summary>
        [ArrayProperty(7)]
        public string? ClientOrderId { get; set; }
        /// <summary>
        /// Side
        /// </summary>
        [ArrayProperty(8)]
        [JsonConverter(typeof(EnumConverter))]
        public OrderSide OrderSide { get; set; }
    }


}
