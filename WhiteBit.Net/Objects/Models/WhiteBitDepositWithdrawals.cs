using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Deposit/withdrawal history
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositWithdrawals
    {
        /// <summary>
        /// ["<c>limit</c>"] Limit
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        /// <summary>
        /// ["<c>offset</c>"] Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }
        /// <summary>
        /// ["<c>records</c>"] Records
        /// </summary>
        [JsonPropertyName("records")]
        public WhiteBitDepositWithdrawal[] Records { get; set; } = Array.Empty<WhiteBitDepositWithdrawal>();
        /// <summary>
        /// ["<c>total</c>"] Total number of results
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    /// <summary>
    /// Deposit/withdrawal info
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositWithdrawal
    {
        /// <summary>
        /// ["<c>address</c>"] Address
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>uniqueId</c>"] Unique id
        /// </summary>
        [JsonPropertyName("uniqueId")]
        public string? UniqueId { get; set; }
        /// <summary>
        /// ["<c>createdAt</c>"] Creation time
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// ["<c>currency</c>"] Asset name
        /// </summary>
        [JsonPropertyName("currency")]
        public string AssetName { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>ticker</c>"] Asset
        /// </summary>
        [JsonPropertyName("ticker")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>method</c>"] Transaction type
        /// </summary>
        [JsonPropertyName("method")]
        public TransactionType TransactionType { get; set; }
        /// <summary>
        /// ["<c>amount</c>"] Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>description</c>"] Description
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        /// <summary>
        /// ["<c>memo</c>"] Memo
        /// </summary>
        [JsonPropertyName("memo")]
        public string? Memo { get; set; }
        /// <summary>
        /// ["<c>fee</c>"] Fee
        /// </summary>
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }
        /// <summary>
        /// ["<c>status</c>"] Status
        /// </summary>
        [JsonPropertyName("status")]
        public TransactionStatus? TransactionStatus { get; set; }
        /// <summary>
        /// ["<c>network</c>"] Network
        /// </summary>
        [JsonPropertyName("network")]
        public string? Network { get; set; }
        /// <summary>
        /// ["<c>transactionHash</c>"] Transaction hash
        /// </summary>
        [JsonPropertyName("transactionHash")]
        public string TransactionHash { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>transactionId</c>"] Transaction id
        /// </summary>
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>details</c>"] Details
        /// </summary>
        [JsonPropertyName("details")]
        public WhiteBitDepositWithdrawalDetails Details { get; set; } = null!;
        /// <summary>
        /// ["<c>confirmations</c>"] Confirmations
        /// </summary>
        [JsonPropertyName("confirmations")]
        public WhiteBitDepositWithdrawalConfirmations Confirmations { get; set; } = null!;
    }

    /// <summary>
    /// Details
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositWithdrawalDetails
    {
        /// <summary>
        /// ["<c>partial</c>"] Partial
        /// </summary>
        [JsonPropertyName("partial")]
        public WhiteBitDepositWithdrawalPartial Partial { get; set; } = null!;
    }

    /// <summary>
    /// Partial
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositWithdrawalPartial
    {
        /// <summary>
        /// ["<c>requestAmount</c>"] Request quantity
        /// </summary>
        [JsonPropertyName("requestAmount")]
        public decimal RequestQuantity { get; set; }
        /// <summary>
        /// ["<c>processedAmount</c>"] Processed quantity
        /// </summary>
        [JsonPropertyName("processedAmount")]
        public decimal ProcessedQuantity { get; set; }
        /// <summary>
        /// ["<c>processedFee</c>"] Processed fee
        /// </summary>
        [JsonPropertyName("processedFee")]
        public decimal ProcessedFee { get; set; }
        /// <summary>
        /// ["<c>normalizeTransaction</c>"] Normalize transaction
        /// </summary>
        [JsonPropertyName("normalizeTransaction")]
        public string? NormalizeTransaction { get; set; }
    }

    /// <summary>
    /// Confirmations
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositWithdrawalConfirmations
    {
        /// <summary>
        /// ["<c>actual</c>"] Actual
        /// </summary>
        [JsonPropertyName("actual")]
        public int Actual { get; set; }
        /// <summary>
        /// ["<c>required</c>"] Required
        /// </summary>
        [JsonPropertyName("required")]
        public int Required { get; set; }
    }


}
