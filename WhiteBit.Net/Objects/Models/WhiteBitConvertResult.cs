using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Convert result
    /// </summary>
    public record WhiteBitConvertResult
    {
        /// <summary>
        /// Final from quantity
        /// </summary>
        [JsonPropertyName("finalGive")]
        public decimal FinalFromQuantity { get; set; }
        /// <summary>
        /// Final to quantity
        /// </summary>
        [JsonPropertyName("finalReceive")]
        public decimal FinalToQuantity { get; set; }
    }


}
