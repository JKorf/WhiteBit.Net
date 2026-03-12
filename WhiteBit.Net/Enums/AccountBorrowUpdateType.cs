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
        /// ["<c>1</c>"] Margin call
        /// </summary>
        [Map("1")]
        MarginCall,
        /// <summary>
        /// ["<c>2</c>"] Liquidation
        /// </summary>
        [Map("2")]
        Liquidation
    }
}
