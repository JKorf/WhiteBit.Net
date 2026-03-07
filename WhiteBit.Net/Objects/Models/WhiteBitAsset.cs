using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Asset info
    /// </summary>
    [SerializationModel]
    public record WhiteBitAsset
    {
        /// <summary>
        /// Asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>name</c>"] Name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>unified_cryptoasset_id</c>"] Unified cryptoasset id
        /// </summary>
        [JsonPropertyName("unified_cryptoasset_id")]
        public long? UnifiedCryptoassetId { get; set; }
        /// <summary>
        /// ["<c>can_withdraw</c>"] Can withdraw
        /// </summary>
        [JsonPropertyName("can_withdraw")]
        public bool CanWithdraw { get; set; }
        /// <summary>
        /// ["<c>can_deposit</c>"] Can deposit
        /// </summary>
        [JsonPropertyName("can_deposit")]
        public bool CanDeposit { get; set; }
        /// <summary>
        /// ["<c>min_withdraw</c>"] Min withdraw
        /// </summary>
        [JsonPropertyName("min_withdraw")]
        public decimal MinWithdraw { get; set; }
        /// <summary>
        /// ["<c>max_withdraw</c>"] Max withdraw
        /// </summary>
        [JsonPropertyName("max_withdraw")]
        public decimal MaxWithdraw { get; set; }
        /// <summary>
        /// ["<c>maker_fee</c>"] Maker fee
        /// </summary>
        [JsonPropertyName("maker_fee")]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// ["<c>taker_fee</c>"] Taker fee
        /// </summary>
        [JsonPropertyName("taker_fee")]
        public decimal TakerFee { get; set; }
        /// <summary>
        /// ["<c>min_deposit</c>"] Min deposit
        /// </summary>
        [JsonPropertyName("min_deposit")]
        public decimal MinDeposit { get; set; }
        /// <summary>
        /// ["<c>max_deposit</c>"] Max deposit
        /// </summary>
        [JsonPropertyName("max_deposit")]
        public decimal MaxDeposit { get; set; }
        /// <summary>
        /// ["<c>currency_precision</c>"] Asset precision
        /// </summary>
        [JsonPropertyName("currency_precision")]
        public decimal AssetPrecision { get; set; }
        /// <summary>
        /// ["<c>is_memo</c>"] Is memo
        /// </summary>
        [JsonPropertyName("is_memo")]
        public bool IsMemo { get; set; }
        /// <summary>
        /// ["<c>networks</c>"] Networks
        /// </summary>
        [JsonPropertyName("networks")]
        public WhiteBitAssetNetworks Networks { get; set; } = null!;
        /// <summary>
        /// ["<c>limits</c>"] Limits
        /// </summary>
        [JsonPropertyName("limits")]
        public WhiteBitAssetLimits Limits { get; set; } = null!;
        /// <summary>
        /// ["<c>confirmations</c>"] Confirmations
        /// </summary>
        [JsonPropertyName("confirmations")]
        public Dictionary<string, int?> Confirmations { get; set; } = null!;
    }

    /// <summary>
    /// Deposit/withdrawal network info
    /// </summary>
    [SerializationModel]
    public record WhiteBitAssetNetworks
    {
        /// <summary>
        /// ["<c>deposits</c>"] Deposits
        /// </summary>
        [JsonPropertyName("deposits")]
        public string[] Deposits { get; set; } = Array.Empty<string>();
        /// <summary>
        /// ["<c>withdraws</c>"] Withdraws
        /// </summary>
        [JsonPropertyName("withdraws")]
        public string[] Withdraws { get; set; } = Array.Empty<string>();
        /// <summary>
        /// ["<c>default</c>"] Default
        /// </summary>
        [JsonPropertyName("default")]
        public string Default { get; set; } = string.Empty;
    }

    /// <summary>
    /// Asset limits
    /// </summary>
    [SerializationModel]
    public record WhiteBitAssetLimits
    {
        /// <summary>
        /// ["<c>deposit</c>"] Deposit
        /// </summary>
        [JsonPropertyName("deposit")]
        public Dictionary<string, WhiteBitAssetLimit> Deposit { get; set; } = null!;
        /// <summary>
        /// ["<c>withdraw</c>"] Withdraw
        /// </summary>
        [JsonPropertyName("withdraw")]
        public Dictionary<string, WhiteBitAssetLimit> Withdraw { get; set; } = null!;
    }

    /// <summary>
    /// 
    /// </summary>
    [SerializationModel]
    public record WhiteBitAssetLimit
    {
        /// <summary>
        /// ["<c>min</c>"] Min
        /// </summary>
        [JsonPropertyName("min")]
        public decimal Min { get; set; }
    }

}
