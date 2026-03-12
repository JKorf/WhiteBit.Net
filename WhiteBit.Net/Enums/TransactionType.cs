using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Transaction type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<TransactionType>))]
    public enum TransactionType
    {
        /// <summary>
        /// ["<c>1</c>"] Deposit
        /// </summary>
        [Map("1")]
        Deposit = 1,
        /// <summary>
        /// ["<c>2</c>"] Withdrawal
        /// </summary>
        [Map("2")]
        Withdrawal = 2
    }
}
