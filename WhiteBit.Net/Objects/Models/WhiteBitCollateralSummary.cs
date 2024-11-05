using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Balance summary
    /// </summary>
    public record WhiteBitCollateralSummary
    {
        /// <summary>
        /// Asset
        /// </summary>
        [JsonPropertyName("asset")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// Balance
        /// </summary>
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }
        /// <summary>
        /// Borrow
        /// </summary>
        [JsonPropertyName("borrow")]
        public decimal Borrow { get; set; }
        /// <summary>
        /// Available without borrow
        /// </summary>
        [JsonPropertyName("availableWithoutBorrow")]
        public decimal AvailableWithoutBorrow { get; set; }
        /// <summary>
        /// Available with borrow
        /// </summary>
        [JsonPropertyName("availableWithBorrow")]
        public decimal AvailableWithBorrow { get; set; }
    }


}
