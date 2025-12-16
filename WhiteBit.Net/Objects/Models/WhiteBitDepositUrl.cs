using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace WhiteBit.Net.Objects.Models
{
    /// <summary>
    /// Fiat deposit url
    /// </summary>
    [SerializationModel]
    public record WhiteBitDepositUrl
    {
        /// <summary>
        /// The deposit url
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }


}
