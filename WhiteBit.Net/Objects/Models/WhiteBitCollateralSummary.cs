using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Balance summary
    /// </summary>
    [SerializationModel]
    public record WhiteBitCollateralSummary
    {
        /// <summary>
        /// ["<c>asset</c>"] Asset
        /// </summary>
        [JsonPropertyName("asset")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>balance</c>"] Balance
        /// </summary>
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }
        /// <summary>
        /// ["<c>borrow</c>"] Borrow
        /// </summary>
        [JsonPropertyName("borrow")]
        public decimal Borrow { get; set; }
        /// <summary>
        /// ["<c>availableWithoutBorrow</c>"] Available without borrow
        /// </summary>
        [JsonPropertyName("availableWithoutBorrow")]
        public decimal AvailableWithoutBorrow { get; set; }
        /// <summary>
        /// ["<c>availableWithBorrow</c>"] Available with borrow
        /// </summary>
        [JsonPropertyName("availableWithBorrow")]
        public decimal AvailableWithBorrow { get; set; }
    }


}
