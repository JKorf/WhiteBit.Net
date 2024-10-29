using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Deposit address info
    /// </summary>
    public record WhiteBitDepositAddressInfo
    {
        /// <summary>
        /// Account
        /// </summary>
        [JsonPropertyName("account")]
        public WhiteBitDepositAddress Account { get; set; } = null!;
        /// <summary>
        /// Fee info
        /// </summary>
        [JsonPropertyName("required")]
        public WhiteBitDepositWithdrawDetails FeeInfo { get; set; } = null!;
    }

    /// <summary>
    /// Address info
    /// </summary>
    public record WhiteBitDepositAddress
    {
        /// <summary>
        /// Address
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// Memo
        /// </summary>
        [JsonPropertyName("memo")]
        public decimal Memo { get; set; }
    }
}
