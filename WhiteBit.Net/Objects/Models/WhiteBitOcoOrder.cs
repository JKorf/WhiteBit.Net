using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// OCO order
    /// </summary>
    public record WhiteBitOcoOrder
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// Stop loss order
        /// </summary>
        [JsonPropertyName("stop_loss")]
        public WhiteBitConditionalOrder StopLossOrder { get; set; } = null!;
        /// <summary>
        /// Take profit order
        /// </summary>
        [JsonPropertyName("take_profit")]
        public WhiteBitConditionalOrder TakeProfitOrder { get; set; } = null!;
    }
}
