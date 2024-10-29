using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Converters;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Deposit/withdrawal info
    /// </summary>
    [JsonConverter(typeof(DepositWithdrawalInfoConverter))]
    public record WhiteBitDepositWithdraw
    {
        /// <summary>
        /// Crypto info
        /// </summary>
        public IEnumerable<WhiteBitDepositWithdrawalCrypto> CryptoInfo { get; set; } = Array.Empty<WhiteBitDepositWithdrawalCrypto>();
        /// <summary>
        /// Fiat infos
        /// </summary>
        public IEnumerable<WhiteBitDepositWithdrawalFiat> FiatInfo { get; set; } = Array.Empty<WhiteBitDepositWithdrawalFiat>();
    }

    /// <summary>
    /// Deposit/withdrawal info
    /// </summary>
    public record WhiteBitDepositWithdrawInfo
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
        /// Providers
        /// </summary>
        [JsonPropertyName("providers")]
        public IEnumerable<string> Providers { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Is depositable
        /// </summary>
        [JsonPropertyName("is_depositable")]
        public bool IsDepositable { get; set; }
        /// <summary>
        /// Is withdrawable
        /// </summary>
        [JsonPropertyName("is_withdrawal")]
        public bool IsWithdrawable { get; set; }
        /// <summary>
        /// Is withdrawal via API allowed
        /// </summary>
        [JsonPropertyName("is_api_withdrawal")]
        public bool IsApiWithdrawable { get; set; }
        /// <summary>
        /// Is deposit via API allowed
        /// </summary>
        [JsonPropertyName("is_api_depositable")]
        public bool IsApiDepositable { get; set; }
    }

    /// <summary>
    /// Crypto deposit/withdrawal info
    /// </summary>
    public record WhiteBitDepositWithdrawalCrypto : WhiteBitDepositWithdrawInfo
    {
        /// <summary>
        /// Deposit
        /// </summary>
        [JsonPropertyName("deposit")]
        public WhiteBitDepositWithdrawDetails Deposit { get; set; } = null!;
        /// <summary>
        /// Withdraw
        /// </summary>
        [JsonPropertyName("withdraw")]
        public WhiteBitDepositWithdrawDetails Withdraw { get; set; } = null!;
    }

    /// <summary>
    /// Crypto deposit/withdrawal info
    /// </summary>
    public record WhiteBitDepositWithdrawalFiat : WhiteBitDepositWithdrawInfo
    {
        /// <summary>
        /// Deposit
        /// </summary>
        [JsonPropertyName("deposit")]
        public Dictionary<string, WhiteBitDepositWithdrawProviderDetails> Deposit { get; set; } = null!;
        /// <summary>
        /// Withdraw
        /// </summary>
        [JsonPropertyName("withdraw")]
        public Dictionary<string, WhiteBitDepositWithdrawProviderDetails> Withdraw { get; set; } = null!;
    }

    /// <summary>
    /// Withdrawal/deposit limits/fees details
    /// </summary>
    public record WhiteBitDepositWithdrawDetails
    {
        /// <summary>
        /// Min quantity
        /// </summary>
        [JsonPropertyName("min_amount")]
        public decimal MinQuantity { get; set; }
        [JsonInclude, JsonPropertyName("minAmount")]
        internal decimal MinAmount { set => MinQuantity = value; }
        /// <summary>
        /// Max quantity
        /// </summary>
        [JsonPropertyName("max_amount")]
        public decimal? MaxQuantity { get; set; }
        [JsonInclude, JsonPropertyName("maxAmount")]
        internal decimal MaxAmount { set => MaxQuantity = value; }
        /// <summary>
        /// Fixed
        /// </summary>
        [JsonPropertyName("fixed")]
        public decimal? Fixed { get; set; }
        [JsonInclude, JsonPropertyName("fixedFee")]
        internal decimal FixedFee { set => Fixed = value; }
        /// <summary>
        /// Flex fees
        /// </summary>
        [JsonPropertyName("flex")]
        public WhiteBitDepositWithdrawInfoFlex? Flex { get; set; }
        [JsonInclude, JsonPropertyName("flexFee")]
        internal WhiteBitDepositWithdrawInfoFlex FlexFee { set => Flex = value; }
    }

    /// <summary>
    /// Provider
    /// </summary>
    public record WhiteBitDepositWithdrawProviderDetails
    {
        /// <summary>
        /// Min quantity
        /// </summary>
        [JsonPropertyName("min_amount")]
        public decimal MinQuantity { get; set; }
        /// <summary>
        /// Max quantity
        /// </summary>
        [JsonPropertyName("max_amount")]
        public decimal? MaxQuantity { get; set; }
        /// <summary>
        /// Fixed
        /// </summary>
        [JsonPropertyName("fixed")]
        public decimal? Fixed { get; set; }
        /// <summary>
        /// Flex
        /// </summary>
        [JsonPropertyName("flex")]
        public WhiteBitDepositWithdrawInfoFlex? Flex { get; set; }
        /// <summary>
        /// Is depositable
        /// </summary>
        [JsonPropertyName("is_depositable")]
        public bool IsDepositable { get; set; }
        /// <summary>
        /// Is api depositable
        /// </summary>
        [JsonPropertyName("is_api_depositable")]
        public bool IsApiDepositable { get; set; }
        /// <summary>
        /// Is withdrawable
        /// </summary>
        [JsonPropertyName("is_withdrawal")]
        public bool IsWithdrawable { get; set; }
        /// <summary>
        /// Is api withdrawable
        /// </summary>
        [JsonPropertyName("is_api_withdrawal")]
        public bool IsApiWithdrawable { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Ticker
        /// </summary>
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; } = string.Empty;
    }

    /// <summary>
    /// Flex fee info
    /// </summary>
    public record WhiteBitDepositWithdrawInfoFlex
    {
        /// <summary>
        /// Min fee
        /// </summary>
        [JsonPropertyName("min_fee")]
        public decimal MinFee { get; set; }
        [JsonInclude, JsonPropertyName("minFee")]
        internal decimal minFee { set => MinFee = value; }
        /// <summary>
        /// Max fee
        /// </summary>
        [JsonPropertyName("max_fee")]
        public decimal MaxFee { get; set; }
        [JsonInclude, JsonPropertyName("maxFee")]
        internal decimal maxFee { set => MaxFee = value; }
        /// <summary>
        /// Percent
        /// </summary>
        [JsonPropertyName("percent")]
        public decimal Percent { get; set; }
    }

    

}
