using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Order type
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Limit
        /// </summary>
        [Map("1", "limit")]
        Limit = 1,
        /// <summary>
        /// Market
        /// </summary>
        [Map("2", "market")]
        Market = 2,
        /// <summary>
        /// Market order with quantity in base asset
        /// </summary>
        [Map("202", "market_stock")]
        MarketBase = 202,
        /// <summary>
        /// Stop limit
        /// </summary>
        [Map("3", "stop_limit")]
        StopLimit = 3,
        /// <summary>
        /// Stop market
        /// </summary>
        [Map("4", "stop_market")]
        StopMarket = 4,
        /// <summary>
        /// Margin limit
        /// </summary>
        [Map("7")]
        MarginLimit = 7,
        /// <summary>
        /// Margin limit
        /// </summary>
        [Map("8")]
        MarginMarket = 8,
        /// <summary>
        /// Margin limit
        /// </summary>
        [Map("9")]
        MarginStopLimit = 9,
        /// <summary>
        /// Margin limit
        /// </summary>
        [Map("10")]
        MarginTriggerStopMarket = 10,
        /// <summary>
        /// Margin limit
        /// </summary>
        [Map("14")]
        MarginNormalization = 14
    }
}
