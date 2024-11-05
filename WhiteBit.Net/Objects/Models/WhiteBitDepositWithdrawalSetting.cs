using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Withdrawal/deposit settings
    /// </summary>
    public record WhiteBitDepositWithdrawalSetting
    {
        /// <summary>
        /// Asset
        /// </summary>
        [JsonPropertyName("ticker")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// Name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Can deposit
        /// </summary>
        [JsonPropertyName("can_deposit")]
        public bool CanDeposit { get; set; }
        /// <summary>
        /// Can withdraw
        /// </summary>
        [JsonPropertyName("can_withdraw")]
        public bool CanWithdraw { get; set; }
        /// <summary>
        /// Deposit info
        /// </summary>
        [JsonPropertyName("deposit")]
        public WhiteBitDepositWithdrawalSettingDetails Deposit { get; set; } = null!;
        /// <summary>
        /// Withdraw info
        /// </summary>
        [JsonPropertyName("withdraw")]
        public WhiteBitDepositWithdrawalSettingDetails Withdraw { get; set; } = null!;
    }

    /// <summary>
    /// Details
    /// </summary>
    public record WhiteBitDepositWithdrawalSettingDetails
    {
        /// <summary>
        /// Min fee amount when flex fee is enabled
        /// </summary>
        [JsonPropertyName("minFlex")]
        public decimal MinFlex { get; set; }
        /// <summary>
        /// Max fee amount when flex fee is enabled
        /// </summary>
        [JsonPropertyName("maxFlex")]
        public decimal MaxFlex { get; set; }
        /// <summary>
        /// Flex fee percentage
        /// </summary>
        [JsonPropertyName("percentFlex")]
        public decimal PercentFlex { get; set; }
        /// <summary>
        /// Fixed fee
        /// </summary>
        [JsonPropertyName("fixed")]
        public decimal Fixed { get; set; }
        /// <summary>
        /// Min quantity
        /// </summary>
        [JsonPropertyName("minAmount")]
        public decimal MinQuantity { get; set; }
        /// <summary>
        /// Max quantity
        /// </summary>
        [JsonPropertyName("maxAmount")]
        public decimal MaxQuantity { get; set; }
    }
}
