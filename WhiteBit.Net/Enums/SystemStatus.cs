using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Status of the system
    /// </summary>
    [JsonConverter(typeof(EnumConverter<SystemStatus>))]
    public enum SystemStatus
    {
        /// <summary>
        /// System operational
        /// </summary>
        [Map("1")]
        Operational,
        /// <summary>
        /// System maintenance
        /// </summary>
        [Map("0")]
        Maintenance
    }
}
