using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Activation condition
    /// </summary>
    [JsonConverter(typeof(EnumConverter<ActivationCondition>))]
    public enum ActivationCondition
    {
        /// <summary>
        /// ["<c>gte</c>"] Price condition higher than x
        /// </summary>
        [Map("gte")]
        GreaterThanOrEqual,
        /// <summary>
        /// ["<c>lte</c>"] Price condition lower than x
        /// </summary>
        [Map("lte")]
        LessThanOrEqu
    }
}
