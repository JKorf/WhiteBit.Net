using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Conditional order type
    /// </summary>
    public enum ConditionalOrderType
    {
        /// <summary>
        /// OTO
        /// </summary>
        [Map("oto")]
        OneTriggersOther,
        /// <summary>
        /// OCO
        /// </summary>
        [Map("oco")]
        OneCancelsOther
    }
}
