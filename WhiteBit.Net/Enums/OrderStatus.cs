using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Order status
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Unfilled
        /// </summary>
        [Map("Open")]
        Open,
        /// <summary>
        /// Canceled
        /// </summary>
        [Map("Canceled", "2")]
        Canceled,
        /// <summary>
        /// Filled
        /// </summary>
        [Map("Filled", "1")]
        Filled
    }
}
