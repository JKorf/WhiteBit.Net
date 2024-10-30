using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Order product type
    /// </summary>
    public enum OrderProductType
    {
        /// <summary>
        /// Spot
        /// </summary>
        [Map("Spot")]
        Spot,
        /// <summary>
        /// Margin
        /// </summary>
        [Map("Margin")]
        Margin,
        /// <summary>
        /// Futures
        /// </summary>
        [Map("Futures")]
        Futures
    }
}
