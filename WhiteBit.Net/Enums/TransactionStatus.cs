using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Transaction status
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        [Map("1", "2", "6", "10", "11", "12", "13", "14", "15", "16", "17")]
        Pending,
        /// <summary>
        /// Success
        /// </summary>
        [Map("3", "7")]
        Success,
        /// <summary>
        /// Canceled
        /// </summary>
        [Map("4", "9")]
        Canceled,
        /// <summary>
        /// Unconfirmed by user
        /// </summary>
        [Map("5")]
        UnconfirmedByUser,
        /// <summary>
        /// Frozen
        /// </summary>
        [Map("31")]
        Frozen,
        /// <summary>
        /// Uncredited
        /// </summary>
        [Map("22")]
        Uncredited,
        /// <summary>
        /// Partial success
        /// </summary>
        [Map("18")]
        PartialSuccess
    }
}
