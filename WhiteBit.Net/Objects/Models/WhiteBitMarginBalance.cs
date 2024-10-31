using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Margin balance
    /// </summary>
    public record WhiteBitMarginBalance
    {
        /// <summary>
        /// Asset
        /// </summary>
        [JsonPropertyName("a")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// Balance
        /// </summary>
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }

        [JsonInclude, JsonPropertyName("B")]
        internal decimal BalanceInt { set => Balance = value; }

        /// <summary>
        /// Borrow
        /// </summary>
        [JsonPropertyName("borrow")]
        public decimal Borrow { get; set; }

        [JsonInclude, JsonPropertyName("b")]
        internal decimal BorrowInt { set => Borrow = value; }

        /// <summary>
        /// Available without borrow
        /// </summary>
        [JsonPropertyName("available_without_borrow")]
        public decimal AvailableWithoutBorrow { get; set; }

        [JsonInclude, JsonPropertyName("av")]
        internal decimal AvailableWithoutBorrowInt { set => AvailableWithoutBorrow = value; }

        /// <summary>
        /// Available with borrow
        /// </summary>
        [JsonPropertyName("available_with_borrow")]
        public decimal AvailableWithBorrow { get; set; }

        [JsonInclude, JsonPropertyName("ab")]
        internal decimal AvailableWithBorrowInt { set => AvailableWithBorrow = value; }
    }
}
