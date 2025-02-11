using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Self Trade Prevention mode
    /// </summary>
    public enum SelfTradePreventionMode
    {
        /// <summary>
        /// None
        /// </summary>
        [Map("no")]
        None,
        /// <summary>
        /// Cancel both pre-existing and new order
        /// </summary>
        [Map("cancel_both")]
        CancelBoth,
        /// <summary>
        /// Cancel the new order
        /// </summary>
        [Map("cancel_new")]
        CancelNew,
        /// <summary>
        /// Cancel the pre-exisiting order
        /// </summary>
        [Map("cancel_old")]
        CancelOld
    }
}
