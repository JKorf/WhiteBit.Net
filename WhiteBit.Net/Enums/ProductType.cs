using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Product type
    /// </summary>
    public enum ProductType
    {
        /// <summary>
        /// Delivery futures
        /// </summary>
        [Map("Futures")]
        Futures,
        /// <summary>
        /// Perpetual
        /// </summary>
        [Map("Perpetual")]
        Perpetual,
        /// <summary>
        /// Options
        /// </summary>
        [Map("Options")]
        Options
    }
}
