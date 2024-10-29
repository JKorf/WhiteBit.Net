using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// 
    /// </summary>
    public record WhiteBitFuturesSymbol
    {
        /// <summary>
        /// Symbol name
        /// </summary>
        [JsonPropertyName("ticker_id")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Base asset
        /// </summary>
        [JsonPropertyName("stock_currency")]
        public string BaseAsset { get; set; } = string.Empty;
        /// <summary>
        /// Quote asset
        /// </summary>
        [JsonPropertyName("money_currency")]
        public string QuoteAsset { get; set; } = string.Empty;
        /// <summary>
        /// Last price
        /// </summary>
        [JsonPropertyName("last_price")]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// Base asset volume
        /// </summary>
        [JsonPropertyName("stock_volume")]
        public decimal BaseVolume { get; set; }
        /// <summary>
        /// Quote asset volume
        /// </summary>
        [JsonPropertyName("money_volume")]
        public decimal QuoteVolume { get; set; }
        /// <summary>
        /// Best bid price
        /// </summary>
        [JsonPropertyName("bid")]
        public decimal BestBidPrice { get; set; }
        /// <summary>
        /// Best ask price
        /// </summary>
        [JsonPropertyName("ask")]
        public decimal BestAskPrice { get; set; }
        /// <summary>
        /// High price
        /// </summary>
        [JsonPropertyName("high")]
        public decimal HighPrice { get; set; }
        /// <summary>
        /// Low price
        /// </summary>
        [JsonPropertyName("low")]
        public decimal LowPrice { get; set; }
        /// <summary>
        /// Product type
        /// </summary>
        [JsonPropertyName("product_type")]
        public ProductType ProductType { get; set; }
        /// <summary>
        /// Open interest
        /// </summary>
        [JsonPropertyName("open_interest")]
        public decimal OpenInterest { get; set; }
        /// <summary>
        /// Index price
        /// </summary>
        [JsonPropertyName("index_price")]
        public decimal IndexPrice { get; set; }
        /// <summary>
        /// Index name
        /// </summary>
        [JsonPropertyName("index_name")]
        public string IndexName { get; set; } = string.Empty;
        /// <summary>
        /// Index asset
        /// </summary>
        [JsonPropertyName("index_currency")]
        public string IndexAsset { get; set; } = string.Empty;
        /// <summary>
        /// Funding rate
        /// </summary>
        [JsonPropertyName("funding_rate")]
        public decimal FundingRate { get; set; }
        /// <summary>
        /// Next funding rate timestamp
        /// </summary>
        [JsonPropertyName("next_funding_rate_timestamp")]
        public DateTime NextFundingRateTime { get; set; }
        /// <summary>
        /// Brackets
        /// </summary>
        [JsonPropertyName("brackets")]
        public Dictionary<string, long> Brackets { get; set; } = new Dictionary<string, long>()!;
        /// <summary>
        /// Max leverage
        /// </summary>
        [JsonPropertyName("max_leverage")]
        public decimal MaxLeverage { get; set; }
    }
}
