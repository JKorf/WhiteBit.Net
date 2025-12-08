using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Hedge mode status
    /// </summary>
    public record WhiteBitHedgeMode
    {
        /// <summary>
        /// In hedge mode
        /// </summary>
        [JsonPropertyName("hedgeMode")]
        public bool HedgeMode { get; set; }
    }


}
