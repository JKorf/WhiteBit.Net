using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Transaction type
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Deposit
        /// </summary>
        [Map("1")]
        Deposit = 1,
        /// <summary>
        /// Withdrawal
        /// </summary>
        [Map("2")]
        Withdrawal = 2
    }
}
