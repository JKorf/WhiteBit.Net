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
        /// ["<c>1</c>"] System operational
        /// </summary>
        [Map("1")]
        Operational,
        /// <summary>
        /// ["<c>0</c>"] System maintenance
        /// </summary>
        [Map("0")]
        Maintenance
    }
}
