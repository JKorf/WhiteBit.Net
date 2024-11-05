using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Deposit/withdrawal history
    /// </summary>
    public record WhiteBitDepositWithdrawals
    {
        /// <summary>
        /// Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        /// <summary>
        /// Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }
        /// <summary>
        /// Records
        /// </summary>
        [JsonPropertyName("records")]
        public IEnumerable<WhiteBitDepositWithdrawal> Records { get; set; } = Array.Empty<WhiteBitDepositWithdrawal>();
        /// <summary>
        /// Total number of results
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    /// <summary>
    /// Deposit/withdrawal info
    /// </summary>
    public record WhiteBitDepositWithdrawal
    {
        /// <summary>
        /// Address
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// Unique id
        /// </summary>
        [JsonPropertyName("uniqueId")]
        public string? UniqueId { get; set; }
        /// <summary>
        /// Creation time
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Asset name
        /// </summary>
        [JsonPropertyName("currency")]
        public string AssetName { get; set; } = string.Empty;
        /// <summary>
        /// Asset
        /// </summary>
        [JsonPropertyName("ticker")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// Transaction type
        /// </summary>
        [JsonPropertyName("method")]
        public TransactionType TransactionType { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        /// <summary>
        /// Memo
        /// </summary>
        [JsonPropertyName("memo")]
        public string? Memo { get; set; }
        /// <summary>
        /// Fee
        /// </summary>
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        [JsonPropertyName("status")]
        public TransactionStatus? TransactionStatus { get; set; }
        /// <summary>
        /// Network
        /// </summary>
        [JsonPropertyName("network")]
        public string? Network { get; set; }
        /// <summary>
        /// Transaction hash
        /// </summary>
        [JsonPropertyName("transactionHash")]
        public string TransactionHash { get; set; } = string.Empty;
        /// <summary>
        /// Transaction id
        /// </summary>
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; } = string.Empty;
        /// <summary>
        /// Details
        /// </summary>
        [JsonPropertyName("details")]
        public WhiteBitDepositWithdrawalDetails Details { get; set; } = null!;
        /// <summary>
        /// Confirmations
        /// </summary>
        [JsonPropertyName("confirmations")]
        public WhiteBitDepositWithdrawalConfirmations Confirmations { get; set; } = null!;
    }

    /// <summary>
    /// Details
    /// </summary>
    public record WhiteBitDepositWithdrawalDetails
    {
        /// <summary>
        /// Partial
        /// </summary>
        [JsonPropertyName("partial")]
        public WhiteBitDepositWithdrawalPartial Partial { get; set; } = null!;
    }

    /// <summary>
    /// Partial
    /// </summary>
    public record WhiteBitDepositWithdrawalPartial
    {
        /// <summary>
        /// Request quantity
        /// </summary>
        [JsonPropertyName("requestAmount")]
        public decimal RequestQuantity { get; set; }
        /// <summary>
        /// Processed quantity
        /// </summary>
        [JsonPropertyName("processedAmount")]
        public decimal ProcessedQuantity { get; set; }
        /// <summary>
        /// Processed fee
        /// </summary>
        [JsonPropertyName("processedFee")]
        public decimal ProcessedFee { get; set; }
        /// <summary>
        /// Normalize transaction
        /// </summary>
        [JsonPropertyName("normalizeTransaction")]
        public string? NormalizeTransaction { get; set; }
    }

    /// <summary>
    /// Confirmations
    /// </summary>
    public record WhiteBitDepositWithdrawalConfirmations
    {
        /// <summary>
        /// Actual
        /// </summary>
        [JsonPropertyName("actual")]
        public int Actual { get; set; }
        /// <summary>
        /// Required
        /// </summary>
        [JsonPropertyName("required")]
        public int Required { get; set; }
    }


}
