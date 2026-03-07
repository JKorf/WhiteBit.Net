using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Code application result
    /// </summary>
    [SerializationModel]
    public record WhiteBitCodeResult
    {
        /// <summary>
        /// ["<c>message</c>"] Message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>ticker</c>"] Asset
        /// </summary>
        [JsonPropertyName("ticker")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>amount</c>"] Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>external_id</c>"] External id
        /// </summary>
        [JsonPropertyName("external_id")]
        public string ExternalId { get; set; } = string.Empty;
    }


}
