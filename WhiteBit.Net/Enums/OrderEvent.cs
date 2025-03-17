using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Order update event
    /// </summary>
    [JsonConverter(typeof(EnumConverter<OrderEvent>))]
    public enum OrderEvent
    {
        /// <summary>
        /// New order
        /// </summary>
        [Map("1")]
        New,
        /// <summary>
        /// Update
        /// </summary>
        [Map("2")]
        Update,
        /// <summary>
        /// Finished
        /// </summary>
        [Map("3")]
        Finished
    }
}
