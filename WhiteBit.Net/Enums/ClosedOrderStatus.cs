using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Status of a closed order
    /// </summary>
    public enum ClosedOrderStatus
    {
        /// <summary>
        /// All, for filtering
        /// </summary>
        [Map("All")]
        All,
        /// <summary>
        /// Canceled
        /// </summary>
        [Map("Canceled")]
        Canceled,
        /// <summary>
        /// Filled
        /// </summary>
        [Map("Filled")]
        Filled
    }
}
