using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Borrow update
    /// </summary>
    public enum AccountBorrowUpdateType
    {
        /// <summary>
        /// Margin call
        /// </summary>
        [Map("1")]
        MarginCall,
        /// <summary>
        /// Liquidation
        /// </summary>
        [Map("2")]
        Liquidation
    }
}
