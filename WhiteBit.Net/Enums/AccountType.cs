using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Account type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<AccountType>))]
    public enum AccountType
    {
        /// <summary>
        /// ["<c>main</c>"] Main account
        /// </summary>
        [Map("main")]
        Main,
        /// <summary>
        /// ["<c>spot</c>"] Spot account
        /// </summary>
        [Map("spot")]
        Spot,
        /// <summary>
        /// ["<c>collateral</c>"] Collateral account
        /// </summary>
        [Map("collateral")]
        Collateral
    }
}
