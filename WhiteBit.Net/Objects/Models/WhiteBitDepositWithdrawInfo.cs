using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WhiteBit.Net.Converters;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Deposit/withdrawal info
    /// </summary>
    [JsonConverter(typeof(DepositWithdrawalInfoConverter))]
    [SerializationModel]
    public record WhiteBitDepositWithdraw
    {
        /// <summary>
        /// Crypto info
        /// </summary>
        public WhiteBitDepositWithdrawalCrypto[] CryptoInfo { get; set; } = Array.Empty<WhiteBitDepositWithdrawalCrypto>();
        /// <summary>
        /// Fiat infos
        /// </summary>
        public WhiteBitDepositWithdrawalFiat[] FiatInfo { get; set; } = Array.Empty<WhiteBitDepositWithdrawalFiat>();
    }

    /// <summary>
    /// Deposit/withdrawal info
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositWithdrawInfo
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
        /// ["<c>providers</c>"] Providers
        /// </summary>
        [JsonPropertyName("providers")]
        public string[] Providers { get; set; } = Array.Empty<string>();
        /// <summary>
        /// ["<c>is_depositable</c>"] Is depositable
        /// </summary>
        [JsonPropertyName("is_depositable")]
        public bool IsDepositable { get; set; }
        /// <summary>
        /// ["<c>is_withdrawal</c>"] Is withdrawable
        /// </summary>
        [JsonPropertyName("is_withdrawal")]
        public bool IsWithdrawable { get; set; }
        /// <summary>
        /// ["<c>is_api_withdrawal</c>"] Is withdrawal via API allowed
        /// </summary>
        [JsonPropertyName("is_api_withdrawal")]
        public bool IsApiWithdrawable { get; set; }
        /// <summary>
        /// ["<c>is_api_depositable</c>"] Is deposit via API allowed
        /// </summary>
        [JsonPropertyName("is_api_depositable")]
        public bool IsApiDepositable { get; set; }
    }

    /// <summary>
    /// Crypto deposit/withdrawal info
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositWithdrawalCrypto : WhiteBitDepositWithdrawInfo
    {
        /// <summary>
        /// ["<c>deposit</c>"] Deposit
        /// </summary>
        [JsonPropertyName("deposit")]
        public WhiteBitDepositWithdrawDetails Deposit { get; set; } = null!;
        /// <summary>
        /// ["<c>withdraw</c>"] Withdraw
        /// </summary>
        [JsonPropertyName("withdraw")]
        public WhiteBitDepositWithdrawDetails Withdraw { get; set; } = null!;
    }

    /// <summary>
    /// Crypto deposit/withdrawal info
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositWithdrawalFiat : WhiteBitDepositWithdrawInfo
    {
        /// <summary>
        /// ["<c>deposit</c>"] Deposit
        /// </summary>
        [JsonPropertyName("deposit")]
        public Dictionary<string, WhiteBitDepositWithdrawProviderDetails> Deposit { get; set; } = null!;
        /// <summary>
        /// ["<c>withdraw</c>"] Withdraw
        /// </summary>
        [JsonPropertyName("withdraw")]
        public Dictionary<string, WhiteBitDepositWithdrawProviderDetails> Withdraw { get; set; } = null!;
    }

    /// <summary>
    /// Withdrawal/deposit limits/fees details
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositWithdrawDetails
    {
        /// <summary>
        /// ["<c>min_amount</c>"] Min quantity
        /// </summary>
        [JsonPropertyName("min_amount")]
        public decimal MinQuantity { get; set; }
        [JsonInclude, JsonPropertyName("minAmount")]
        internal decimal MinAmount { set => MinQuantity = value; }
        /// <summary>
        /// ["<c>max_amount</c>"] Max quantity
        /// </summary>
        [JsonPropertyName("max_amount")]
        public decimal? MaxQuantity { get; set; }
        [JsonInclude, JsonPropertyName("maxAmount")]
        internal decimal MaxAmount { set => MaxQuantity = value; }
        /// <summary>
        /// ["<c>fixed</c>"] Fixed
        /// </summary>
        [JsonPropertyName("fixed")]
        public decimal? Fixed { get; set; }
        [JsonInclude, JsonPropertyName("fixedFee")]
        internal decimal FixedFee { set => Fixed = value; }
        /// <summary>
        /// ["<c>flex</c>"] Flex fees
        /// </summary>
        [JsonPropertyName("flex")]
        public WhiteBitDepositWithdrawInfoFlex? Flex { get; set; }
        [JsonInclude, JsonPropertyName("flexFee")]
        internal WhiteBitDepositWithdrawInfoFlex FlexFee { set => Flex = value; }
    }

    /// <summary>
    /// Provider
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositWithdrawProviderDetails
    {
        /// <summary>
        /// ["<c>min_amount</c>"] Min quantity
        /// </summary>
        [JsonPropertyName("min_amount")]
        public decimal MinQuantity { get; set; }
        /// <summary>
        /// ["<c>max_amount</c>"] Max quantity
        /// </summary>
        [JsonPropertyName("max_amount")]
        public decimal? MaxQuantity { get; set; }
        /// <summary>
        /// ["<c>fixed</c>"] Fixed
        /// </summary>
        [JsonPropertyName("fixed")]
        public decimal? Fixed { get; set; }
        /// <summary>
        /// ["<c>flex</c>"] Flex
        /// </summary>
        [JsonPropertyName("flex")]
        public WhiteBitDepositWithdrawInfoFlex? Flex { get; set; }
        /// <summary>
        /// ["<c>is_depositable</c>"] Is depositable
        /// </summary>
        [JsonPropertyName("is_depositable")]
        public bool IsDepositable { get; set; }
        /// <summary>
        /// ["<c>is_api_depositable</c>"] Is api depositable
        /// </summary>
        [JsonPropertyName("is_api_depositable")]
        public bool IsApiDepositable { get; set; }
        /// <summary>
        /// ["<c>is_withdrawal</c>"] Is withdrawable
        /// </summary>
        [JsonPropertyName("is_withdrawal")]
        public bool IsWithdrawable { get; set; }
        /// <summary>
        /// ["<c>is_api_withdrawal</c>"] Is api withdrawable
        /// </summary>
        [JsonPropertyName("is_api_withdrawal")]
        public bool IsApiWithdrawable { get; set; }
        /// <summary>
        /// ["<c>name</c>"] Name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>ticker</c>"] Ticker
        /// </summary>
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; } = string.Empty;
    }

    /// <summary>
    /// Flex fee info
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositWithdrawInfoFlex
    {
        /// <summary>
        /// ["<c>min_fee</c>"] Min fee
        /// </summary>
        [JsonPropertyName("min_fee")]
        public decimal MinFee { get; set; }
        [JsonInclude, JsonPropertyName("minFee")]
        internal decimal minFee { set => MinFee = value; }
        /// <summary>
        /// ["<c>max_fee</c>"] Max fee
        /// </summary>
        [JsonPropertyName("max_fee")]
        public decimal MaxFee { get; set; }
        [JsonInclude, JsonPropertyName("maxFee")]
        internal decimal maxFee { set => MaxFee = value; }
        /// <summary>
        /// ["<c>percent</c>"] Percent
        /// </summary>
        [JsonPropertyName("percent")]
        public decimal Percent { get; set; }
    }

    

}
