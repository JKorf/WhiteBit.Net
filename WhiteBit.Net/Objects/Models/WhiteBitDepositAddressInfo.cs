using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Deposit address info
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositAddressInfo
    {
        /// <summary>
        /// ["<c>account</c>"] Account
        /// </summary>
        [JsonPropertyName("account")]
        public WhiteBitDepositAddress Account { get; set; } = null!;
        /// <summary>
        /// ["<c>required</c>"] Fee info
        /// </summary>
        [JsonPropertyName("required")]
        public WhiteBitDepositWithdrawDetails FeeInfo { get; set; } = null!;
    }

    /// <summary>
    /// Address info
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositAddress
    {
        /// <summary>
        /// ["<c>address</c>"] Address
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>memo</c>"] Memo
        /// </summary>
        [JsonPropertyName("memo")]
        public decimal Memo { get; set; }
    }
}
