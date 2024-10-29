using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Asset info
    /// </summary>
    public record WhiteBitAsset
    {
        /// <summary>
        /// Asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// Name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Unified cryptoasset id
        /// </summary>
        [JsonPropertyName("unified_cryptoasset_id")]
        public long? UnifiedCryptoassetId { get; set; }
        /// <summary>
        /// Can withdraw
        /// </summary>
        [JsonPropertyName("can_withdraw")]
        public bool CanWithdraw { get; set; }
        /// <summary>
        /// Can deposit
        /// </summary>
        [JsonPropertyName("can_deposit")]
        public bool CanDeposit { get; set; }
        /// <summary>
        /// Min withdraw
        /// </summary>
        [JsonPropertyName("min_withdraw")]
        public decimal MinWithdraw { get; set; }
        /// <summary>
        /// Max withdraw
        /// </summary>
        [JsonPropertyName("max_withdraw")]
        public decimal MaxWithdraw { get; set; }
        /// <summary>
        /// Maker fee
        /// </summary>
        [JsonPropertyName("maker_fee")]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// Taker fee
        /// </summary>
        [JsonPropertyName("taker_fee")]
        public decimal TakerFee { get; set; }
        /// <summary>
        /// Min deposit
        /// </summary>
        [JsonPropertyName("min_deposit")]
        public decimal MinDeposit { get; set; }
        /// <summary>
        /// Max deposit
        /// </summary>
        [JsonPropertyName("max_deposit")]
        public decimal MaxDeposit { get; set; }
        /// <summary>
        /// Asset precision
        /// </summary>
        [JsonPropertyName("currency_precision")]
        public decimal AssetPrecision { get; set; }
        /// <summary>
        /// Is memo
        /// </summary>
        [JsonPropertyName("is_memo")]
        public bool IsMemo { get; set; }
        /// <summary>
        /// Networks
        /// </summary>
        [JsonPropertyName("networks")]
        public WhiteBitAssetNetworks Networks { get; set; } = null!;
        /// <summary>
        /// Limits
        /// </summary>
        [JsonPropertyName("limits")]
        public WhiteBitAssetLimits Limits { get; set; } = null!;
        /// <summary>
        /// Confirmations
        /// </summary>
        [JsonPropertyName("confirmations")]
        public Dictionary<string, int?> Confirmations { get; set; } = null!;
    }

    /// <summary>
    /// Deposit/withdrawal network info
    /// </summary>
    public record WhiteBitAssetNetworks
    {
        /// <summary>
        /// Deposits
        /// </summary>
        [JsonPropertyName("deposits")]
        public IEnumerable<string> Deposits { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Withdraws
        /// </summary>
        [JsonPropertyName("withdraws")]
        public IEnumerable<string> Withdraws { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Default
        /// </summary>
        [JsonPropertyName("default")]
        public string Default { get; set; } = string.Empty;
    }

    /// <summary>
    /// Asset limits
    /// </summary>
    public record WhiteBitAssetLimits
    {
        /// <summary>
        /// Deposit
        /// </summary>
        [JsonPropertyName("deposit")]
        public Dictionary<string, WhiteBitAssetLimit> Deposit { get; set; } = null!;
        /// <summary>
        /// Withdraw
        /// </summary>
        [JsonPropertyName("withdraw")]
        public Dictionary<string, WhiteBitAssetLimit> Withdraw { get; set; } = null!;
    }

    /// <summary>
    /// 
    /// </summary>
    public record WhiteBitAssetLimit
    {
        /// <summary>
        /// Min
        /// </summary>
        [JsonPropertyName("min")]
        public decimal Min { get; set; }
    }

}
