using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// WhiteBit code
    /// </summary>
    [SerializationModel]
    public record WhiteBitCode
    {
        /// <summary>
        /// ["<c>code</c>"] Generated WhiteBit code
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>message</c>"] Message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>external_id</c>"] External id
        /// </summary>
        [JsonPropertyName("external_id")]
        public string ExternalId { get; set; } = string.Empty;
    }


}
