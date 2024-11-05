using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Trade info
    /// </summary>
    public record WhiteBitTrade
    {
        /// <summary>
        /// Trade id
        /// </summary>
        [JsonPropertyName("tradeID")]
        public long TradeId { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        /// <summary>
        /// Quote volume
        /// </summary>
        [JsonPropertyName("base_volume")]
        // THE VOLUME PROPERTIES ARE INCORRECTLY INVERSED IN THE API RESPONSE
        public decimal QuoteVolume { get; set; }
        /// <summary>
        /// Base volume
        /// </summary>
        // THE VOLUME PROPERTIES ARE INCORRECTLY INVERSED IN THE API RESPONSE
        [JsonPropertyName("quote_volume")]
        public decimal BaseVolume { get; set; }
        /// <summary>
        /// Trade timestamp
        /// </summary>
        [JsonPropertyName("trade_timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Trade side
        /// </summary>
        [JsonPropertyName("type")]
        public OrderSide Side { get; set; }
    }


}
