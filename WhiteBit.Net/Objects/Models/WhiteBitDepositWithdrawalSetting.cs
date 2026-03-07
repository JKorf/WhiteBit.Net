using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Withdrawal/deposit settings
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositWithdrawalSetting
    {
        /// <summary>
        /// ["<c>ticker</c>"] Asset
        /// </summary>
        [JsonPropertyName("ticker")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>name</c>"] Name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>can_deposit</c>"] Can deposit
        /// </summary>
        [JsonPropertyName("can_deposit")]
        public bool CanDeposit { get; set; }
        /// <summary>
        /// ["<c>can_withdraw</c>"] Can withdraw
        /// </summary>
        [JsonPropertyName("can_withdraw")]
        public bool CanWithdraw { get; set; }
        /// <summary>
        /// ["<c>deposit</c>"] Deposit info
        /// </summary>
        [JsonPropertyName("deposit")]
        public WhiteBitDepositWithdrawalSettingDetails Deposit { get; set; } = null!;
        /// <summary>
        /// ["<c>withdraw</c>"] Withdraw info
        /// </summary>
        [JsonPropertyName("withdraw")]
        public WhiteBitDepositWithdrawalSettingDetails Withdraw { get; set; } = null!;
    }

    /// <summary>
    /// Details
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositWithdrawalSettingDetails
    {
        /// <summary>
        /// ["<c>minFlex</c>"] Min fee amount when flex fee is enabled
        /// </summary>
        [JsonPropertyName("minFlex")]
        public decimal MinFlex { get; set; }
        /// <summary>
        /// ["<c>maxFlex</c>"] Max fee amount when flex fee is enabled
        /// </summary>
        [JsonPropertyName("maxFlex")]
        public decimal MaxFlex { get; set; }
        /// <summary>
        /// ["<c>percentFlex</c>"] Flex fee percentage
        /// </summary>
        [JsonPropertyName("percentFlex")]
        public decimal PercentFlex { get; set; }
        /// <summary>
        /// ["<c>fixed</c>"] Fixed fee
        /// </summary>
        [JsonPropertyName("fixed")]
        public decimal Fixed { get; set; }
        /// <summary>
        /// ["<c>minAmount</c>"] Min quantity
        /// </summary>
        [JsonPropertyName("minAmount")]
        public decimal MinQuantity { get; set; }
        /// <summary>
        /// ["<c>maxAmount</c>"] Max quantity
        /// </summary>
        [JsonPropertyName("maxAmount")]
        public decimal MaxQuantity { get; set; }
    }
}
