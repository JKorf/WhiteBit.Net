using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Trade role
    /// </summary>
    public enum TradeRole
    {
        /// <summary>
        /// Maker
        /// </summary>
        [Map("1")]
        Maker,
        /// <summary>
        /// Taker
        /// </summary>
        [Map("2")]
        Taker
    }
}
