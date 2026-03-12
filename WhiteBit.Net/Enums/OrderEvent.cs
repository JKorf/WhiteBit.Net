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
        /// ["<c>1</c>"] New order
        /// </summary>
        [Map("1")]
        New,
        /// <summary>
        /// ["<c>2</c>"] Update
        /// </summary>
        [Map("2")]
        Update,
        /// <summary>
        /// ["<c>3</c>"] Finished
        /// </summary>
        [Map("3")]
        Finished
    }
}
