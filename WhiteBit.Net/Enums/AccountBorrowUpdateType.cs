using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Borrow update
    /// </summary>
    [JsonConverter(typeof(EnumConverter<AccountBorrowUpdateType>))]
    public enum AccountBorrowUpdateType
    {
        /// <summary>
        /// Margin call
        /// </summary>
        [Map("1")]
        MarginCall,
        /// <summary>
        /// Liquidation
        /// </summary>
        [Map("2")]
        Liquidation
    }
}
