using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Kill switch info
    /// </summary>
    public record WhiteBitKillSwitch
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Start time
        /// </summary>
        [JsonPropertyName("startTime")]
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// Cancellation time
        /// </summary>
        [JsonPropertyName("cancellationTime")]
        public DateTime? CancellationTime { get; set; }
        /// <summary>
        /// Product types the kill switch is applied to
        /// </summary>
        [JsonPropertyName("types")]
        public IEnumerable<OrderProductType>? OrderProductTypes { get; set; }
    }


}
