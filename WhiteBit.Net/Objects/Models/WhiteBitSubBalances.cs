using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Sub balances
    /// </summary>
    public record WhiteBitSubBalances
    {
        /// <summary>
        /// Asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// Main
        /// </summary>
        [JsonPropertyName("main")]
        public decimal Main { get; set; }
        /// <summary>
        /// Spot
        /// </summary>
        [JsonPropertyName("spot")]
        public decimal Spot { get; set; }
        /// <summary>
        /// Collateral
        /// </summary>
        [JsonPropertyName("collateral")]
        public decimal Collateral { get; set; }
    }
}
