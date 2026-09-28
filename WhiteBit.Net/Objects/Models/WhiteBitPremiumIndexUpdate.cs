using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Premium index info update
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<WhiteBitPremiumIndexUpdate>))]
    public record WhiteBitPremiumIndexUpdate
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [ArrayProperty(0)]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Mark price
        /// </summary>
        [ArrayProperty(1)]
        public decimal MarkPrice { get; set; }
        /// <summary>
        /// Index price
        /// </summary>
        [ArrayProperty(2)]
        public decimal IndexPrice { get; set; }
        /// <summary>
        /// Funding rate
        /// </summary>
        [ArrayProperty(3)]
        public decimal FundingRate { get; set; }
        /// <summary>
        /// Next funding timestamp
        /// </summary>
        [ArrayProperty(4)]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime NextFundingTime { get; set; }
    }
}
