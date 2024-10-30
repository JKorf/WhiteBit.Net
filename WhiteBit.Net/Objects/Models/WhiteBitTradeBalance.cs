using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Trade balance
    /// </summary>
    public record WhiteBitTradeBalance
    {
        /// <summary>
        /// Asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// Available balance
        /// </summary>
        [JsonPropertyName("available")]
        public decimal Available { get; set; }
        /// <summary>
        /// Frozen balance
        /// </summary>
        [JsonPropertyName("freeze")]
        public decimal Frozen { get; set; }
    }


}
