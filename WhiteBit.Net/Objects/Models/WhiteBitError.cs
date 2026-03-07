using CryptoExchange.Net.Converters.SystemTextJson;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Error
    /// </summary>
    [SerializationModel]
    public record WhiteBitError
    {
        /// <summary>
        /// ["<c>code</c>"] Error code
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }
        /// <summary>
        /// ["<c>message</c>"] Error message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>errors</c>"] Error messages
        /// </summary>
        [JsonPropertyName("errors")]
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
