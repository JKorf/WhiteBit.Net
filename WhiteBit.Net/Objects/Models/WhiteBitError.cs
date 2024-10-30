using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Error
    /// </summary>
    public record WhiteBitError
    {
        /// <summary>
        /// Error code
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }
        /// <summary>
        /// Error message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// Error messages
        /// </summary>
        [JsonPropertyName("errors")]
        public Dictionary<string, IEnumerable<string>>? Errors { get; set; }
    }
}
