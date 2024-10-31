using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Closed order filter
    /// </summary>
    public enum ClosedOrderFilter
    {
        /// <summary>
        /// Limit and market orders
        /// </summary>
        [Map("0")]
        LimitAndMarket = 0,
        /// <summary>
        /// Only limit orders
        /// </summary>
        [Map("1")]
        Limit = 1,
        /// <summary>
        /// Only market orders
        /// </summary>
        [Map("2")]
        Market = 2
    }
}
