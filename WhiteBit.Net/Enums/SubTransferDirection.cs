using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Sub account transfer direction
    /// </summary>
    public enum SubTransferDirection
    {
        /// <summary>
        /// From sub account to main
        /// </summary>
        [Map("sub_to_main")]
        FromSubAccount,
        /// <summary>
        /// From main to sub account
        /// </summary>
        [Map("main_to_sub")]
        ToSubAccount
    }
}
