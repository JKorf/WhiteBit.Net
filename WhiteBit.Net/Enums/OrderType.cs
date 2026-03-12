using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Order type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<OrderType>))]
    public enum OrderType
    {
        /// <summary>
        /// ["<c>1</c>"] Limit
        /// </summary>
        [Map("1", "limit")]
        Limit = 1,
        /// <summary>
        /// ["<c>2</c>"] Market
        /// </summary>
        [Map("2", "market")]
        Market = 2,
        /// <summary>
        /// ["<c>202</c>"] Market order with quantity in base asset
        /// </summary>
        [Map("202", "market_stock", "stock market")]
        MarketBase = 202,
        /// <summary>
        /// ["<c>3</c>"] Stop limit
        /// </summary>
        [Map("3", "stop_limit", "stop limit")]
        StopLimit = 3,
        /// <summary>
        /// ["<c>4</c>"] Stop market
        /// </summary>
        [Map("4", "stop_market", "stop market")]
        StopMarket = 4,
        /// <summary>
        /// ["<c>7</c>"] Margin limit
        /// </summary>
        [Map("7", "margin limit")]
        CollateralLimit = 7,
        /// <summary>
        /// ["<c>8</c>"] Margin market
        /// </summary>
        [Map("8", "margin market")]
        CollateralMarket = 8,
        /// <summary>
        /// ["<c>9</c>"] Margin stop limit
        /// </summary>
        [Map("9", "margin stop limit")]
        CollateralStopLimit = 9,
        /// <summary>
        /// ["<c>10</c>"] Margin trigger stop market
        /// </summary>
        [Map("10", "trigger stop market", "trigger margin market")]
        CollateralTriggerStopMarket = 10,
        /// <summary>
        /// ["<c>14</c>"] Margin normalization
        /// </summary>
        [Map("14", "normalization")]
        CollateralNormalization = 14
    }
}
