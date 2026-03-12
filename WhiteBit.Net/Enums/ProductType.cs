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
        /// ["<c>Futures</c>"] Delivery futures
        /// </summary>
        [Map("Futures")]
        Futures,
        /// <summary>
        /// ["<c>Perpetual</c>"] Perpetual
        /// </summary>
        [Map("Perpetual")]
        Perpetual,
        /// <summary>
        /// ["<c>Options</c>"] Options
        /// </summary>
        [Map("Options")]
        Options
    }
}
