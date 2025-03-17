using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Sub account transfer direction
    /// </summary>
    [JsonConverter(typeof(EnumConverter<SubTransferDirection>))]
    public enum SubTransferDirection
    {
        /// <summary>
        /// From sub account to main
        /// </summary>
        [Map("sub_to_main")]
        FromSubAccount,
        /// <summary>
        /// From main to sub account
        /// </summary>
        [Map("main_to_sub")]
        ToSubAccount
    }
}
