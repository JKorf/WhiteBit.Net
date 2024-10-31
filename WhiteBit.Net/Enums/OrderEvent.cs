using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Order update event
    /// </summary>
    public enum OrderEvent
    {
        /// <summary>
        /// New order
        /// </summary>
        [Map("1")]
        New,
        /// <summary>
        /// Update
        /// </summary>
        [Map("2")]
        Update,
        /// <summary>
        /// Finished
        /// </summary>
        [Map("3")]
        Finished
    }
}
