using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace WhiteBit.Net.Enums
{
    /// <summary>
    /// Self Trade Prevention mode
    /// </summary>
    [JsonConverter(typeof(EnumConverter<SelfTradePreventionMode>))]
    public enum SelfTradePreventionMode
    {
        /// <summary>
        /// ["<c>no</c>"] None
        /// </summary>
        [Map("no")]
        None,
        /// <summary>
        /// ["<c>cancel_both</c>"] Cancel both pre-existing and new order
        /// </summary>
        [Map("cancel_both")]
        CancelBoth,
        /// <summary>
        /// ["<c>cancel_new</c>"] Cancel the new order
        /// </summary>
        [Map("cancel_new")]
        CancelNew,
        /// <summary>
        /// ["<c>cancel_old</c>"] Cancel the pre-exisiting order
        /// </summary>
        [Map("cancel_old")]
        CancelOld
    }
}
