using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Margin balance
    /// </summary>
    [SerializationModel]
    public record WhiteBitMarginBalance
    {
        /// <summary>
        /// ["<c>a</c>"] Asset
        /// </summary>
        [JsonPropertyName("a")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>balance</c>"] Balance
        /// </summary>
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }

        [JsonInclude, JsonPropertyName("B")]
        internal decimal BalanceInt { set => Balance = value; }

        /// <summary>
        /// ["<c>borrow</c>"] Borrow
        /// </summary>
        [JsonPropertyName("borrow")]
        public decimal Borrow { get; set; }

        [JsonInclude, JsonPropertyName("b")]
        internal decimal BorrowInt { set => Borrow = value; }

        /// <summary>
        /// ["<c>available_without_borrow</c>"] Available without borrow
        /// </summary>
        [JsonPropertyName("available_without_borrow")]
        public decimal AvailableWithoutBorrow { get; set; }

        [JsonInclude, JsonPropertyName("av")]
        internal decimal AvailableWithoutBorrowInt { set => AvailableWithoutBorrow = value; }

        /// <summary>
        /// ["<c>available_with_borrow</c>"] Available with borrow
        /// </summary>
        [JsonPropertyName("available_with_borrow")]
        public decimal AvailableWithBorrow { get; set; }

        [JsonInclude, JsonPropertyName("ab")]
        internal decimal AvailableWithBorrowInt { set => AvailableWithBorrow = value; }
    }
}
