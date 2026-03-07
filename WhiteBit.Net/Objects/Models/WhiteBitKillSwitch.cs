using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;
using WhiteBit.Net.Enums;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Kill switch info
    /// </summary>
    [SerializationModel]
    public record WhiteBitKillSwitch
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>startTime</c>"] Start time
        /// </summary>
        [JsonPropertyName("startTime")]
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// ["<c>cancellationTime</c>"] Cancellation time
        /// </summary>
        [JsonPropertyName("cancellationTime")]
        public DateTime? CancellationTime { get; set; }
        /// <summary>
        /// ["<c>types</c>"] Product types the kill switch is applied to
        /// </summary>
        [JsonPropertyName("types")]
        public OrderProductType[]? OrderProductTypes { get; set; }
    }


}
