using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Trade role
    /// </summary>
    [JsonConverter(typeof(EnumConverter<TradeRole>))]
    public enum TradeRole
    {
        /// <summary>
        /// Maker
        /// </summary>
        [Map("1")]
        Maker,
        /// <summary>
        /// Taker
        /// </summary>
        [Map("2")]
        Taker
    }
}
