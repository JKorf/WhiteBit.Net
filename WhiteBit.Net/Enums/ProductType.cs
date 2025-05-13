using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Product type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<ProductType>))]
    public enum ProductType
    {
        /// <summary>
        /// Delivery futures
        /// </summary>
        [Map("Futures")]
        Futures,
        /// <summary>
        /// Perpetual
        /// </summary>
        [Map("Perpetual")]
        Perpetual,
        /// <summary>
        /// Options
        /// </summary>
        [Map("Options")]
        Options
    }
}
