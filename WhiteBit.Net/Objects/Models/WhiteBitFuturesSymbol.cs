using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SerializationModel]
    public record WhiteBitFuturesSymbol
    {
        /// <summary>
        /// ["<c>ticker_id</c>"] Symbol name
        /// </summary>
        [JsonPropertyName("ticker_id")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>stock_currency</c>"] Base asset
        /// </summary>
        [JsonPropertyName("stock_currency")]
        public string BaseAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>money_currency</c>"] Quote asset
        /// </summary>
        [JsonPropertyName("money_currency")]
        public string QuoteAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>last_price</c>"] Last price
        /// </summary>
        [JsonPropertyName("last_price")]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// ["<c>stock_volume</c>"] Base asset volume
        /// </summary>
        [JsonPropertyName("stock_volume")]
        public decimal BaseVolume { get; set; }
        /// <summary>
        /// ["<c>money_volume</c>"] Quote asset volume
        /// </summary>
        [JsonPropertyName("money_volume")]
        public decimal QuoteVolume { get; set; }
        /// <summary>
        /// ["<c>bid</c>"] Best bid price
        /// </summary>
        [JsonPropertyName("bid")]
        public decimal BestBidPrice { get; set; }
        /// <summary>
        /// ["<c>ask</c>"] Best ask price
        /// </summary>
        [JsonPropertyName("ask")]
        public decimal BestAskPrice { get; set; }
        /// <summary>
        /// ["<c>high</c>"] High price
        /// </summary>
        [JsonPropertyName("high")]
        public decimal HighPrice { get; set; }
        /// <summary>
        /// ["<c>low</c>"] Low price
        /// </summary>
        [JsonPropertyName("low")]
        public decimal LowPrice { get; set; }
        /// <summary>
        /// ["<c>product_type</c>"] Product type
        /// </summary>
        [JsonPropertyName("product_type")]
        public ProductType ProductType { get; set; }
        /// <summary>
        /// ["<c>open_interest</c>"] Open interest
        /// </summary>
        [JsonPropertyName("open_interest")]
        public decimal OpenInterest { get; set; }
        /// <summary>
        /// ["<c>index_price</c>"] Index price
        /// </summary>
        [JsonPropertyName("index_price")]
        public decimal IndexPrice { get; set; }
        /// <summary>
        /// ["<c>index_name</c>"] Index name
        /// </summary>
        [JsonPropertyName("index_name")]
        public string IndexName { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>index_currency</c>"] Index asset
        /// </summary>
        [JsonPropertyName("index_currency")]
        public string IndexAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>funding_rate</c>"] Funding rate
        /// </summary>
        [JsonPropertyName("funding_rate")]
        public decimal FundingRate { get; set; }
        /// <summary>
        /// ["<c>next_funding_rate_timestamp</c>"] Next funding rate timestamp
        /// </summary>
        [JsonPropertyName("next_funding_rate_timestamp")]
        public DateTime NextFundingRateTime { get; set; }
        /// <summary>
        /// ["<c>brackets</c>"] Brackets
        /// </summary>
        [JsonPropertyName("brackets")]
        public Dictionary<string, long> Brackets { get; set; } = new Dictionary<string, long>()!;
        /// <summary>
        /// ["<c>max_leverage</c>"] Max leverage
        /// </summary>
        [JsonPropertyName("max_leverage")]
        public decimal MaxLeverage { get; set; }
    }
}
