using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Liquidation status
    /// </summary>
    public enum LiquidationStatus
    {
        /// <summary>
        /// Margin call
        /// </summary>
        [Map("margin_call")]
        MarginCall,
        /// <summary>
        /// Liquidation
        /// </summary>
        [Map("liquidation")]
        Liquidation,
    }
}
