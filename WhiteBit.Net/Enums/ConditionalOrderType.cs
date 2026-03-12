using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Conditional order type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<ConditionalOrderType>))]
    public enum ConditionalOrderType
    {
        /// <summary>
        /// ["<c>oto</c>"] OTO
        /// </summary>
        [Map("oto")]
        OneTriggersOther,
        /// <summary>
        /// ["<c>oco</c>"] OCO
        /// </summary>
        [Map("oco")]
        OneCancelsOther
    }
}
